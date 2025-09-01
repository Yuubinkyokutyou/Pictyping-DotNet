/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ExchangeCodeRequest } from '../models/ExchangeCodeRequest';
import type { LoginRequest } from '../models/LoginRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class AuthService {
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthCrossDomainLogin({
        token,
        returnUrl,
    }: {
        token?: string,
        returnUrl?: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/cross-domain-login',
            query: {
                'token': token,
                'returnUrl': returnUrl,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthRedirectToLegacy({
        targetPath,
    }: {
        targetPath?: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/redirect-to-legacy',
            query: {
                'targetPath': targetPath,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthLogin({
        body,
    }: {
        body?: LoginRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/login',
            body: body,
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthGoogleLogin({
        returnUrl,
    }: {
        returnUrl?: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/google/login',
            query: {
                'returnUrl': returnUrl,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthGoogleProcess(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/google/process',
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthExchangeCode({
        body,
    }: {
        body?: ExchangeCodeRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/exchange-code',
            body: body,
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthMe(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/me',
        });
    }
}
