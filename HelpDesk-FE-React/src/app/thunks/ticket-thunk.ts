import { createAsyncThunk } from "@reduxjs/toolkit";
import { createTicketAsync, deleteTicketAsync, getAllTicketsAsync, getTicketByIdAsync, updateTicketAsync } from "../../services/ticket-service";
import type { TicketCreate, TicketFilter, UpdateTicketPayload } from "../../interfaces/ticket";

export const fetchTickets = createAsyncThunk(
    '/ticket/fetchTicket',
    async (filter: TicketFilter, thunkAPI) => {
        try {
            return await getAllTicketsAsync(filter);
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)

export const createTicket = createAsyncThunk(
    '/ticket/createTicket',
    async (request: TicketCreate, thunkAPI) => {
        try {
            return await createTicketAsync(request);
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message.toString());
        }
    }
)

export const fetchTicketById = createAsyncThunk(
    '/ticket/fetchTicketById',
    async (id: number, thunkAPI) => {
        try {
            return await getTicketByIdAsync(id);
        } catch (error) {
            return thunkAPI.rejectWithValue(error.message);
        }
    }
)

export const updateTicket = createAsyncThunk(
    '/ticket/updateTicket',
    async (params: { id: number, body: UpdateTicketPayload }, thunkApi) => {
        try {
            return await updateTicketAsync(params.id, params.body);
        } catch (error) {
            return thunkApi.rejectWithValue(error.message);
        }
    }
)

export const deleteTicket = createAsyncThunk(
    '/ticket/deleteTicket',
    async (id: number, thunkApi) => {
        try {
            return await deleteTicketAsync(id);
        } catch (error) {
            return thunkApi.rejectWithValue(error.message);
        }
    }
)