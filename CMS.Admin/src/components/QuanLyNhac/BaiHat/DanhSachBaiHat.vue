﻿<template>
    <v-col cols="12" class="pa-0">
        <v-card>
            <v-card-text>
                <h2>Danh sách bài hát</h2>
                <v-row>
                    <v-col cols="4" class="pb-0">
                        <v-text-field v-model="searchParamsBaiHat.keyworlds"
                                      hide-details
                                      append-icon="search"
                                      label="Tìm kiếm"
                                      placeholder="Tìm kiếm theo tên chuyên mục..."></v-text-field>
                    </v-col>
                    <v-col cols="3" class="pb-0">
                        <v-autocomplete v-model="searchParamsBaiHat.albumID"
                                        :items="dsAlbum" clearable hide-details
                                        label="Album"
                                        placeholder="Chọn album..."
                                        item-text="TenAlbum"
                                        item-value="AlbumID">
                        </v-autocomplete>
                    </v-col>
                    <v-col cols="3" class="pb-0">
                        <v-autocomplete v-model="searchParamsBaiHat.theLoaiID"
                                        :items="dsTheLoai" clearable hide-details
                                        label="Thể loại"
                                        placeholder="Chọn thể loại..."
                                        item-text="TenTheLoai"
                                        item-value="TheLoaiID">
                        </v-autocomplete>
                    </v-col>
                    <v-col cols="2" class="pb-0">
                        <v-btn @click="showModalThemSua(false, {})" color="primary" style="margin-top: 30px; float: right" small><v-icon small>add</v-icon> Thêm Mới</v-btn>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="3" class="pt-1">
                        <v-datepicker v-model="searchParamsBaiHat.ngayDangTu"
                                      hide-details
                                      label="Đăng từ ngày"></v-datepicker>
                    </v-col>
                    <v-col cols="3" class="pt-1">
                        <v-datepicker v-model="searchParamsBaiHat.ngayDangDen"
                                      hide-details
                                      label="Đến ngày"></v-datepicker>
                    </v-col>
                    <v-col cols="4" class="pt-1">
                        <v-autocomplete v-model="searchParamsBaiHat.caSyID"
                                        :items="dsCaSy" clearable hide-details
                                        label="Ca sỹ thể hiện"
                                        placeholder="Chọn ca sỹ..."
                                        item-text="HoTen"
                                        item-value="CaSyID">
                        </v-autocomplete>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12" class="pt-0">
                        <v-data-table :headers="tableHeader"
                                      :items="dsBaiHat"
                                      @update:options="getDataFromApi"
                                      :options.sync="searchParamsBaiHat"
                                      :server-items-length="searchParamsBaiHat.totalItems"
                                      :loading="loadingTable"
                                      class="elevation-1">
                            <v-progress-linear height="2" slot="progress" color="blue" indeterminate></v-progress-linear>
                            <template v-slot:body="{ item }">
                                <tbody>
                                    <tr v-for="(item, index) in dsBaiHat" :key="index">
                                        <td class="text-center">{{ index + 1 }}</td>
                                        <td class="text-center">{{ item.TenBaiHat }}</td>
                                        <td class="text-center">{{ item.NgayDang | moment("DD/MM/YYYY hh:mm") }}</td>
                                        <td class="text-center">{{ item.TheLoai ? item.TheLoai.TenTheLoai : ""}}</td>
                                        <td class="text-center">
                                            <span v-for="(casy, idx) in item.CaSy_BaiHat">
                                                {{ casy.CaSy ? (casy.CaSy.BietDanh ? casy.CaSy.BietDanh : casy.CaSy.HoTen ) :""}}
                                                <span v-if="idx != item.CaSy_BaiHat.length-1">, </span>
                                            </span>
                                        </td>
                                        <td class="text-center">
                                            <span v-for="(album, idx) in item.Album_BaiHat">
                                                {{ album.Album ? album.Album.TenAlbum : ""  }}
                                                <span v-if="idx != item.Album_BaiHat.length-1">, </span>
                                            </span>
                                        </td>
                                        <td class="text-center"><a @click="capNhatTrangThai(item)">{{ item.HienThiTrangChu == true? "Hiện" : "Ẩn" }}</a></td>
                                        <td>
                                            <v-layout nowrap style="place-content: center">
                                                <v-btn text icon small @click="showModalThemSua(true, item)" class="ma-0">
                                                    <v-icon small>edit</v-icon>
                                                </v-btn>
                                                <v-btn text icon small color="red" class="ma-0" @click="confirmDelete(item)">
                                                    <v-icon small>delete</v-icon>
                                                </v-btn>
                                            </v-layout>
                                        </td>
                                    </tr>
                                </tbody>

                            </template>
                        </v-data-table>
                    </v-col>
                </v-row>
            </v-card-text>
        </v-card>
        <v-dialog v-model="dialogConfirmDelete" max-width="290">
            <v-card>
                <v-card-title class="headline">Xác nhận xóa</v-card-title>
                <v-card-text class="pt-3">Bạn có chắc chắn muốn xóa?</v-card-text>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn @click.native="dialogConfirmDelete=false" text>Hủy</v-btn>
                    <v-btn color="red darken-1" @click.native="deleteBaiHat" text>Xác Nhận</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <them-sua-bai-hat ref="themSuaBaiHat" @save='getDataFromApi(searchParamsBaiHat)'></them-sua-bai-hat>
    </v-col>
</template>
<script lang="ts">
    import { Vue } from 'vue-property-decorator';
    import BaiHatApi, { BaiHatApiSearchParams } from '@/apiResources/BaiHatApi';
    import { BaiHat } from '@/models/BaiHat';
    import ThemSuaBaiHat from './ThemSuaBaiHat.vue';
    import AlbumApi, { AlbumApiSearchParams } from '../../../apiResources/AlbumApi';
    import CaSyApi, { CaSyApiSearchParams } from '../../../apiResources/CaSyApi';
    import TheLoaiApi, { TheLoaiApiSearchParams } from '../../../apiResources/TheLoaiApi';
    import { Album } from '../../../models/Album';
    import { TheLoai } from '../../../models/TheLoai';
    import { CaSy } from '../../../models/CaSy';

    export default Vue.extend({
        components: {
            ThemSuaBaiHat
        },
        data() {
            return {
                dsBaiHat: [] as BaiHat[],
                dsAlbum: [] as Album[],
                dsTheLoai: [] as TheLoai[],
                dsCaSy: [] as CaSy[],
                tableHeader: [
                    { text: 'STT', value: '#', align: 'center', sortable: true },
                    { text: 'Tên bài', value: 'HoTen', align: 'center', sortable: true },
                    { text: 'Ngày đăng', value: 'TenBaiHat', align: 'center', sortable: true },
                    { text: 'Thể loại', value: 'TenBaiHat', align: 'center', sortable: true },
                    { text: 'Ca sỹ thể hiện', value: 'TrangThai', align: 'center', sortable: true },
                    { text: 'Album', value: 'HienThiTrangChu', align: 'center', sortable: true },
                    { text: 'Trạng thái', value: 'HienThiTrangChu', align: 'center', sortable: true },
                    { text: 'Thao tác', value: '#', align: 'center', sortable: false },
                ],
                searchParamsBaiHat: { includeEntities: true, itemsPerPage: 10 } as BaiHatApiSearchParams,
                searchParamsAlbum: { includeEntities: true, itemsPerPage: 0 } as AlbumApiSearchParams,
                searchParamsCaSy: { includeEntities: true, itemsPerPage: 0 } as CaSyApiSearchParams,
                searchParamsTheLoai: { includeEntities: true, itemsPerPage: 0 } as TheLoaiApiSearchParams,
                loadingTable: false,
                selectedBaiHat: {} as BaiHat,
                dialogConfirmDelete: false,
            }
        },
        watch: {
        },
        created() {
            this.getDanhSachAlbum()
            this.getDanhSachCaSy()
            this.getDanhSachTheLoai()
        },
        methods: {
            getDataFromApi(searchParamsBaiHat: BaiHatApiSearchParams): void {
                this.loadingTable = true;
                BaiHatApi.search(searchParamsBaiHat).then(res => {
                    this.dsBaiHat = res.Data;
                    this.searchParamsBaiHat.totalItems = res.Pagination.totalItems;
                    this.loadingTable = false;
                });
            },
            showModalThemSua(isUpdate: boolean, item: any) {
                (this.$refs.themSuaBaiHat as any).show(isUpdate, item);
            },
            getDanhSachAlbum() {
                AlbumApi.search(this.searchParamsAlbum).then(res => {
                    this.dsAlbum = res.Data
                })
            },
            getDanhSachCaSy() {
                CaSyApi.search(this.searchParamsCaSy).then(res => {
                    this.dsCaSy = res.Data
                })
            },
            getDanhSachTheLoai() {
                TheLoaiApi.search(this.searchParamsTheLoai).then(res => {
                    this.dsTheLoai = res.Data
                })
            },
            capNhatTrangThai(item: BaiHat) {
                item.HienThiTrangChu = !item.HienThiTrangChu;
                item.Album_BaiHat = undefined as any;
                item.TheLoai = undefined as any;
                item.CaSy_BaiHat = undefined as any;
                this.loadingTable = true
                BaiHatApi.update(item.BaiHatID, item).then(res => {
                    this.loadingTable = false;
                    this.getDataFromApi(this.searchParamsBaiHat)
                }).catch(res => {
                    this.loadingTable = false;
                    this.$snotify.error('Cập nhật bài hát thất bại!');
                });
            },
            confirmDelete(chuyenMuc: BaiHat): void {
                this.selectedBaiHat = chuyenMuc;
                this.dialogConfirmDelete = true;
            },
            deleteBaiHat(): void {
                BaiHatApi.delete(this.selectedBaiHat.BaiHatID).then(res => {
                    this.$snotify.success('Xóa thành công!');
                    this.getDataFromApi(this.searchParamsBaiHat);
                    this.dialogConfirmDelete = false;
                }).catch(res => {
                    this.$snotify.error('Xóa thất bại!');
                });
            },
        }
    });
</script>

