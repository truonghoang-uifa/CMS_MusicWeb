﻿

import { HTTP } from '@/HTTPServices';
import { BaseApi } from '@/apiResources/BaseApi';
import { PaginatedResponse, Pagination } from '@/apiResources/PaginatedResponse';
import { Album_BaiHat } from '@/models/Album_BaiHat';
export interface Album_BaiHatApiSearchParams extends Pagination {
    keyworlds?: string;
    albumID?: number
}

class Album_BaiHatApi extends BaseApi {
    
    search(album_BaiHatsearchParams: Album_BaiHatApiSearchParams): Promise<PaginatedResponse<Album_BaiHat>> {
        return new Promise<PaginatedResponse<Album_BaiHat>>((resolve: any, reject: any) => {
            HTTP.get(`api/album_BaiHat`, { params: album_BaiHatsearchParams })
            .then((response) => {
                resolve(response.data);
            }).catch((error) => {
                reject(error);
            })
        });
    }
    insert(album_BaiHat: Album_BaiHat): Promise<Album_BaiHat> {
        return new Promise<Album_BaiHat>((resolve: any, reject: any) => {
            HTTP.post(`api/album_BaiHat`,album_BaiHat)
            .then((response) => {
                resolve(response.data);
            }).catch((error) => {
                reject(error);
            })
        });
    }
    update(albumID: number, baiHatID: number, album_BaiHat: Album_BaiHat): Promise<Album_BaiHat> {
        return new Promise<Album_BaiHat>((resolve: any, reject: any) => {
            HTTP.put(`api/album_BaiHat/${albumID}/${baiHatID}`, album_BaiHat)
                .then((response) => {
                    resolve(response.data);
                }).catch((error) => {
                    reject(error);
                })
        });
    }
}
export default new Album_BaiHatApi();
