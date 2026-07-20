import {configureStore} from "@reduxjs/toolkit";
import authReducer from './slices/auth-slice';
import ticketReducer from './slices/ticket-slice';

const store = configureStore({
    reducer: {
        auth: authReducer,
        ticket: ticketReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export default store;