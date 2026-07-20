export interface AuditLog {
    id: number;
    ticketId: number;
    changedBy: number;
    changedByName: string;
    action: string;
    fieldName: string | null;
    oldValue: string | null;
    newValue: string | null;
    reason: string | null;
    createdAt: string;
}