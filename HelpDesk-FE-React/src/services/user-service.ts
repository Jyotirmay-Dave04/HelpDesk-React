import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { PagedResponse } from "../interfaces/paged-response";
import type { AssignAgentRequest, ChangeRolePayload, UserFilter, UserListItem } from "../interfaces/user";

export const getAllUsersAsync = async (filter: UserFilter): Promise<ApiResponse<PagedResponse<UserListItem>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<UserListItem>>>(ApiEndpoints.USERS.ALL, filter);
    return response.data;
}

export const changeUserRoleAsync = async (userId: number, payload: ChangeRolePayload): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.put<ApiResponse<boolean>>(ApiEndpoints.USERS.CHANGE_ROLE(userId), payload);
    return response.data;
}

export const deleteUserAsync = async (userId: number): Promise<ApiResponse<Boolean>> => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(ApiEndpoints.USERS.DELETE(userId));
    return response.data;
}

export const getAssinableAgentsAsync = async (request: AssignAgentRequest): Promise<ApiResponse<PagedResponse<UserListItem>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<UserListItem>>>(ApiEndpoints.USERS.AGENTS, request);
    return response.data;
}