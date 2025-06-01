/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { UpdateRatingRequest } from '../models/UpdateRatingRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class RankingService {
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiRanking({
        count = 100,
    }: {
        count?: number,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Ranking',
            query: {
                'count': count,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiRankingUser({
        userId,
    }: {
        userId: number,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Ranking/user/{userId}',
            path: {
                'userId': userId,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static putApiRankingUser({
        userId,
        body,
    }: {
        userId: number,
        body?: UpdateRatingRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Ranking/user/{userId}',
            path: {
                'userId': userId,
            },
            body: body,
        });
    }
}
