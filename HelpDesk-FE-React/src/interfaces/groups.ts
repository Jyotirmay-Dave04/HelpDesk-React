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