import { createContext, useContext, useEffect, useRef, useState, type ReactNode } from "react";
import * as signalR from '@microsoft/signalr';
import { useAppDispatch, useAppSelector } from "../hooks/hooks";
import type { NotificationResponse } from "../../interfaces/notification";
import { notificationReceived } from "../slices/notification-slice";
import type { TicketResponse } from "../../interfaces/ticket";
import { ticketCreatedRealTime, ticketUpdatedRealTime } from "../slices/ticket-slice";

const HUB_URL = import.meta.env.VITE_NOTIFICATION_HUB_URL;

const SignalRContext = createContext<signalR.HubConnection | null>(null);

export function useHubConnection() {
    return useContext(SignalRContext);
}

interface SignalRProviderProps {
    children: ReactNode;
}

export function SignalRProvider({ children }: SignalRProviderProps) {
    const dispatch = useAppDispatch();
    const { user, token } = useAppSelector(state => state.auth);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const startedRef = useRef(false);

    useEffect(() => {
        if (!user) {
            connection?.stop();
            setConnection(null);
            startedRef.current = false;
            return;
        }

        if (startedRef.current) return;
        startedRef.current = true;

        const conn = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .build();

        conn.on('ReceiveNotification', (notification: NotificationResponse) => {
            dispatch(notificationReceived(notification));
        });

        conn.on('RecieveNewTicket', (ticket: TicketResponse) => {
            dispatch(ticketCreatedRealTime(ticket));
        });

        conn.on('ReciveUpdatedTicket', (ticket: TicketResponse) => {
            dispatch(ticketUpdatedRealTime(ticket));
        });

        conn.start()
            .then(() => setConnection(conn))
            .catch((err) => console.error("SignalR connection failed: ", err));

        return () => {
            conn.stop();
            startedRef.current = false;
        }
    }, [user, token]);

    return <SignalRContext.Provider value={connection}>{children}</SignalRContext.Provider>
}