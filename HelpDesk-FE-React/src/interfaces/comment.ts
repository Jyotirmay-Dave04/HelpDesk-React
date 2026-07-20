export interface Comment {
    id: number;
    ticketId: number;
    authorId: number;
    authorName: string;
    body: string;
    isInternal: boolean;
    createdAt: string;
}

export interface CreateCommentPayload {
    ticketId: number;
    body: string;
    isInternal: boolean;
}