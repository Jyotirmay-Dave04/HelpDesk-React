import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { NotificationResponse } from "../../interfaces/notification";
import { fetchNotifications, fetchUnReadCount, markAllNotificationRead, markNotificationRead } from "../thunks/notification-thunk";

interface NotificationState {
    notifications: NotificationResponse[];
    unreadCount: number;
    page: number;
    totalPages: number;
    loading: boolean;
}

const initialState: NotificationState = {
    notifications: [],
    unreadCount: 0,
    page: 1,
    totalPages: 1,
    loading: false,
};

const notificationSlice = createSlice({
    name: 'notification',
    initialState,
    reducers: {
        // Called by useNotificationHub when a real-time push arrives over SignalR.
        notificationReceived(state, action: PayloadAction<NotificationResponse>) {
            state.notifications.unshift(action.payload);
            state.unreadCount += 1;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchNotifications.pending, (state) => {
                state.loading = true;
            })
            .addCase(fetchNotifications.fulfilled, (state, action) => {
                state.loading = false;
                const isFirstPage = action.meta.arg.page === 1;
                state.notifications = isFirstPage
                    ? action.payload.data.items
                    : [...state.notifications, ...action.payload.data.items]
                state.page = action.payload.data.page;
                state.totalPages = action.payload.data.totalPages;
            })
            .addCase(fetchNotifications.rejected, (state) => {
                state.loading = false;
            })
            .addCase(fetchUnReadCount.fulfilled, (state, action) => {
                state.unreadCount = action.payload.data;
            })
            .addCase(markNotificationRead.fulfilled, (state, action) => {
                const wasUnread = state.notifications.some((n) => n.id === action.payload.id && !n.isRead);
                state.notifications = state.notifications.filter((n) => n.id !== action.payload.id);
                if (wasUnread) {
                    state.unreadCount = Math.max(0, state.unreadCount - 1);
                }
            })
            .addCase(markAllNotificationRead.fulfilled, (state) => {
                state.notifications = [];
                state.unreadCount = 0;
            });
    },
});

export const { notificationReceived } = notificationSlice.actions;
export default notificationSlice.reducer;