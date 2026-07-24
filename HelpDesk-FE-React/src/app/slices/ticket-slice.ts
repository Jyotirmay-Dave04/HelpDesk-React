import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { TicketListItem, TicketResponse } from "../../interfaces/ticket";
import { createTicket, deleteTicket, fetchTicketById, fetchTickets, updateTicket } from "../thunks/ticket-thunk";

interface TicketState {
    tickets: TicketListItem[];
    ticketCount: number;
    selectedTicket: TicketResponse | null;
    newTickets: TicketResponse | null;
    updatedTickets: TicketResponse | null;
    loading: boolean;
    fetchMessage: string | null;
    createMessage: string | null;
    updateMessage: string | null;
    deleteMessage: string | null;
    error: string | null;
}

const initialState: TicketState = {
    tickets: [],
    ticketCount: 0,
    selectedTicket: null,
    loading: false,
    newTickets: null,
    updatedTickets: null,
    fetchMessage: null,
    createMessage: null,
    updateMessage: null,
    deleteMessage: null,
    error: null
}

export const ticketSlice = createSlice({
    name: 'ticket',
    initialState,
    reducers: {
        clearCreateMessage: (state) => { state.createMessage = null; },
        clearSelectedTicket: (state) => { state.selectedTicket = null; },
        ticketCreatedRealTime: (state, action: PayloadAction<TicketResponse>) => {
            state.newTickets = action.payload;
        },
        ticketUpdatedRealTime: (state, action: PayloadAction<TicketResponse>) => {
            state.updatedTickets = action.payload;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchTickets.pending, (state) => {
                state.tickets = [];
                state.fetchMessage = null;
                state.error = null;
                state.loading = true;
            })
            .addCase(fetchTickets.fulfilled, (state, action) => {
                state.tickets = action.payload.data.items;
                state.ticketCount = action.payload.data.totalCount;
                state.fetchMessage = action.payload.message;
                state.loading = false;
                state.newTickets = null;
                state.updatedTickets = null;
            })
            .addCase(fetchTickets.rejected, (state, action) => {
                state.error = action.payload.toString() ?? "Something went wrong";
                state.loading = false;
            })

            .addCase(createTicket.pending, (state) => {
                state.createMessage = null;
                state.error = null;
                state.loading = true;
            })
            .addCase(createTicket.fulfilled, (state, action) => {
                state.createMessage = action.payload.message;
                state.loading = false;
            })
            .addCase(createTicket.rejected, (state, action) => {
                state.error = action.payload.toString() ?? "Something went wrong";
                state.loading = false;
            })

            .addCase(fetchTicketById.pending, (state) => {
                state.selectedTicket = null;
                state.fetchMessage = null;
                state.error = null;
                state.loading = true;
            })
            .addCase(fetchTicketById.fulfilled, (state, action) => {
                state.selectedTicket = action.payload.data;
                state.fetchMessage = action.payload.message;
                state.loading = false;
            })
            .addCase(fetchTicketById.rejected, (state, action) => {
                state.error = action.payload.toString() ?? "Something went wrong";
                state.loading = false;
            })

            .addCase(updateTicket.pending, (state) => {
                state.error = null;
                state.updateMessage = null;
                state.loading = true;
            })
            .addCase(updateTicket.fulfilled, (state, action) => {
                state.updateMessage = action.payload.message;
                state.selectedTicket = action.payload.data;
                state.loading = false;
            })
            .addCase(updateTicket.rejected, (state, action) => {
                state.error = action.payload.toString() ?? "Something went wrong";
                state.loading = false;
            })

            .addCase(deleteTicket.pending, (state) => {
                state.error = null;
                state.deleteMessage = null;
                state.loading = true;
            })
            .addCase(deleteTicket.fulfilled, (state, action) => {
                state.deleteMessage = action.payload.message;
                state.loading = false;
            })
            .addCase(deleteTicket.rejected, (state, action) => {
                state.error = action.payload.toString() ?? "Something went wrong";
                state.loading = false;
            });
    },
});

export const {
    clearCreateMessage,
    clearSelectedTicket,
    ticketCreatedRealTime,
    ticketUpdatedRealTime
} = ticketSlice.actions;

export default ticketSlice.reducer;