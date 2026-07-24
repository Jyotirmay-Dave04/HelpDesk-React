import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { NotificationRequestFilter, NotificationResponse } from "../interfaces/notification";
import type { PagedResponse } from "../interfaces/paged-response";

export const getNotificationsAsync = async (filter: NotificationRequestFilter): Promise<ApiResponse<PagedResponse<NotificationResponse>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<NotificationResponse>>>(ApiEndpoints.NOTIFICATIONS.BASE, filter);
    return response.data;
}

export const getUnreadCountAsync = async (): Promise<ApiResponse<number>> => {
    const response = await axiosInstance.get<ApiResponse<number>>(ApiEndpoints.NOTIFICATIONS.UNREAD_COUNT);
    return response.data;
}

export const markAsReadAsync = async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.put<ApiResponse<boolean>>(ApiEndpoints.NOTIFICATIONS.MARK_READ(id));
    return response.data;
}

export const markAllAsReadAsync = async (): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.put<ApiResponse<boolean>>(ApiEndpoints.NOTIFICATIONS.MARK_ALL_READ);
    return response.data;
}