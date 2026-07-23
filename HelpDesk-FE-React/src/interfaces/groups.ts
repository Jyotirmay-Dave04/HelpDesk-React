export interface Group {
    id: number;
    name: string;
}

export interface Category {
    id: number;
    name: string;
    groupId: number;
}

export interface SubCategory {
    id: number;
    name: string;
    categoryId: number;
}

export interface CreateGroupPayload {
    name: string;
}

export interface UpdateGroupPayload {
    name: string;
}

export interface CreateCategoryPayload {
    name: string;
    groupId: number;
}

export interface UpdateCategoryPayload {
    name: string;
}

export interface CreateSubCategoryPayload {
    name: string;
    categoryId: number;
}

export interface UpdateSubCategoryPayload {
    name: string;
}

export interface GetPagedGroup {
    page: number;
    pageSize: number;
    search?: string;
}

export interface GetPagedCategory {
    groupId: number;
    page: number;
    pageSize: number;
    search?: string;
}

export interface GetPagedSubCategory {
    categoryId: number;
    page: number;
    pageSize: number;
    search?: string;
}

export interface EntityFormValues {
    name: string;
}

export type EntityLevel = 'group' | 'category' | 'subCategory';