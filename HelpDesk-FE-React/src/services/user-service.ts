import axiosInstance from "../axios/axios-instance";
import { ApiEndpoints } from "../constants/api-endpoints";
import type { ApiResponse } from "../interfaces/api-response";
import type { PagedResponse } from "../interfaces/paged-response";
import type { AssignAgentRequest, UserListItem } from "../interfaces/user";

export const getAssinableAgentsAsync = async (request: AssignAgentRequest): Promise<ApiResponse<PagedResponse<UserListItem>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<UserListItem>>>(ApiEndpoints.USERS.AGENTS, request);
    return response.data;
}