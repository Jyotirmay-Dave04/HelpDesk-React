import { createAsyncThunk } from "@reduxjs/toolkit";
import type { NotificationRequestFilter } from "../../interfaces/notification";
import { getNotificationsAsync, getUnreadCountAsync, markAllAsReadAsync, markAsReadAsync } from "../../services/notification-service";

export const fetchNotifications = createAsyncThunk(
    'notification/fetchNotifications',
    async (filter: NotificationRequestFilter, thunkAPI) => {
        try {
            return await getNotificationsAsync(filter);
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)

export const fetchUnReadCount = createAsyncThunk(
    'notification/fetchUnReadCount',
    async (_: void, thunkAPI) => {
        try {
            return await getUnreadCountAsync();
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)

export const markNotificationRead = createAsyncThunk(
    'notification/markNotificationRead',
    async (id: number, thunkAPI) => {
        try {
            const response =  await markAsReadAsync(id);
            return { id, message: response.message}
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)

export const markAllNotificationRead = createAsyncThunk(
    'notification/markAllNotificationRead',
    async (_: void, thunkAPI) => {
        try {
            return await markAllAsReadAsync();
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)