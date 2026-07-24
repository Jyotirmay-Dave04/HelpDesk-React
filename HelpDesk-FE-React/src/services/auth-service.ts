import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { LoginRequest, LoginResponse, RegisterRequest } from "../interfaces/login";

export const loginAsync = async (creads: LoginRequest): Promise<ApiResponse<LoginResponse>> => {
    const response = await axiosInstance.post<ApiResponse<LoginResponse>>(ApiEndpoints.AUTH.LOGIN, creads);
    return response.data;
}

export const registerAsync = async (creads: RegisterRequest): Promise<ApiResponse<LoginResponse>> => {
    const response = await axiosInstance.post<ApiResponse<LoginResponse>>(ApiEndpoints.AUTH.REGISTER, creads);
    return response.data;
}