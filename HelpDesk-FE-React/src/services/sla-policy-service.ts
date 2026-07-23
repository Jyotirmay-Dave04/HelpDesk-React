import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { SlaPolicyResponse, UpdateSlaPolicyPayload } from "../interfaces/sla-policy";
import type { Priority } from "../types/priority";

export const getAllSlaPolicies = async (): Promise<ApiResponse<SlaPolicyResponse[]>> => {
    const response = await axiosInstance.get<ApiResponse<SlaPolicyResponse[]>>(ApiEndpoints.SLAPOLICY.BASE);
    return response.data;
}

export const updateSlaPolicy = async (priority: Priority, payload: UpdateSlaPolicyPayload): Promise<ApiResponse<SlaPolicyResponse>> => {
    const response = await axiosInstance.put<ApiResponse<SlaPolicyResponse>>(ApiEndpoints.SLAPOLICY.UPDATE(priority), payload);
    return response.data;
}