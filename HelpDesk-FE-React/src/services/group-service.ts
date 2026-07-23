import axiosInstance from "../axios/axios-instance"
import { ApiEndpoints } from "../constants/api-endpoints"
import type { ApiResponse } from "../interfaces/api-response"
import type { Category, CreateCategoryPayload, CreateGroupPayload, CreateSubCategoryPayload, GetPagedCategory, GetPagedGroup, GetPagedSubCategory, Group, SubCategory, UpdateCategoryPayload, UpdateGroupPayload, UpdateSubCategoryPayload } from "../interfaces/groups"
import type { PagedResponse } from "../interfaces/paged-response"

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

export const createGroup = async (payload: CreateGroupPayload): Promise<ApiResponse<Group>> => {
    const response = await axiosInstance.post<ApiResponse<Group>>(ApiEndpoints.GROUPS.CREATE, payload);
    return response.data;
}

export const createCategory = async (payload: CreateCategoryPayload): Promise<ApiResponse<Category>> => {
    const response = await axiosInstance.post<ApiResponse<Category>>(ApiEndpoints.CATEGORIES.CREATE, payload);
    return response.data;
}

export const createSubCategory = async (payload: CreateSubCategoryPayload): Promise<ApiResponse<SubCategory>> => {
    const response = await axiosInstance.post<ApiResponse<SubCategory>>(ApiEndpoints.SUBCATEGORIES.CREATE, payload);
    return response.data;
}

export const updateGroup = async (id: number, payload: UpdateGroupPayload): Promise<ApiResponse<Group>> => {
    const response = await axiosInstance.put<ApiResponse<Group>>(ApiEndpoints.GROUPS.UPDATE(id), payload);
    return response.data;
}

export const updateCategory = async (id: number, payload: UpdateCategoryPayload): Promise<ApiResponse<Category>> => {
    const response = await axiosInstance.put<ApiResponse<Category>>(ApiEndpoints.CATEGORIES.UPDATE(id), payload);
    return response.data;
}

export const updateSubCategory = async (id: number, payload: UpdateSubCategoryPayload): Promise<ApiResponse<SubCategory>> => {
    const response = await axiosInstance.put<ApiResponse<SubCategory>>(ApiEndpoints.SUBCATEGORIES.UPDATE(id), payload);
    return response.data;
}

export const deleteGroup = async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(ApiEndpoints.GROUPS.DELETE(id));
    return response.data;
}

export const deleteCategory = async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(ApiEndpoints.CATEGORIES.DELETE(id));
    return response.data;
}

export const deleteSubCategory = async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await axiosInstance.delete<ApiResponse<boolean>>(ApiEndpoints.SUBCATEGORIES.DELETE(id));
    return response.data;
}

export const getPagedGroups = async (payload: GetPagedGroup): Promise<ApiResponse<PagedResponse<Group>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<Group>>>(ApiEndpoints.GROUPS.PAGED, payload);
    return response.data;
}

export const getPagedCategories = async (payload: GetPagedCategory): Promise<ApiResponse<PagedResponse<Category>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<Category>>>(ApiEndpoints.CATEGORIES.BY_GROUP_PAGED(payload.groupId), payload);
    return response.data;
}

export const getPagedSubCategories = async (payload: GetPagedSubCategory): Promise<ApiResponse<PagedResponse<SubCategory>>> => {
    const response = await axiosInstance.post<ApiResponse<PagedResponse<SubCategory>>>(ApiEndpoints.SUBCATEGORIES.BY_CATEGORY_PAGED(payload.categoryId), payload);
    return response.data;
}