import axiosInstance from "../axios/axios-instance"
import { ApiEndpoints } from "../constants/api-endpoints"
import type { ApiResponse } from "../interfaces/api-response"
import type { Category, Group, SubCategory } from "../interfaces/groups"

export const getGroups = async (): Promise<ApiResponse<Group[]>> => {
    const response = await axiosInstance.get<ApiResponse<Group[]>>(ApiEndpoints.GROUPS.BASE);
    return response.data;
}

export const getCategories = async (groupId: number): Promise<ApiResponse<Category[]>> => {
    const response = await axiosInstance.get<ApiResponse<Category[]>>(ApiEndpoints.CATEGORIES.BY_GROUP(groupId));
    return response.data;
}

export const getSubCategories = async (id: number): Promise<ApiResponse<SubCategory[]>> => {
    const response = await axiosInstance.get<ApiResponse<SubCategory[]>>(ApiEndpoints.SUBCATEGORIES.BY_CATEGORY(id));
    return response.data;
}