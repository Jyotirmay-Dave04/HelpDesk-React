import { useAppDispatch, useAppSelector } from "../app/hooks/hooks";
import { useCallback, useEffect, useState } from "react";
import { Alert, Box, Button, CircularProgress, Grid, Stack, Typography } from "@mui/material";
import TicketInfoCard from "../components/ticket-details/TicketInfoCard";
import CommentsSection from "../components/ticket-details/CommentSection";
import ActivityTimeline from "../components/ticket-details/AuditLogTimeLine";
import { fetchTicketById } from "../app/thunks/ticket-thunk";
import type { AuditLog } from "../interfaces/audit-log";
import { toast } from "../utils/Toast";
import { getAuditLogsByTicketIdAsync } from "../services/audit-log-service";
import { clearSelectedTicket } from "../app/slices/ticket-slice";
import { createCommentAsync, getCommentsByTicketIdAsync } from "../services/comment-service";
import { UserRole } from "../types/user-role";
import type { Comment, CreateCommentPayload } from "../interfaces/comment";
import { useNavigate, useParams } from "react-router-dom";
import TicketActionsBar from "../components/ticket-details/TicketActionBar";
import { useHubConnection } from "../app/provider/SignalRProvider";

const TicketDetailsPage = () => {
    const { ticketId } = useParams();
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const connection = useHubConnection();

    const [commentLoading, setCommentLoading] = useState(false);
    const [comments, setComments] = useState<Comment[]>([]);

    const [activityLoading, setActivityLoading] = useState(false);
    const [activities, setActivities] = useState<AuditLog[]>([]);

    const currentUser = useAppSelector((state) => state.auth.user);
    const { updatedTickets } = useAppSelector(state => state.ticket);
    const { loading: ticketLoading, error: ticketError, selectedTicket: ticket } = useAppSelector(state => state.ticket);

    useEffect(() => {
        if (!ticketId) return;

        dispatch(fetchTicketById(Number(ticketId)));
        fetchTicketActivity(Number(ticketId));
        fetchTicketComments(Number(ticketId));

        return () => {
            dispatch(clearSelectedTicket());
            setActivities([]);
            setComments([]);
        };
    }, [ticketId, dispatch]);

    useEffect(() => {
        if (!connection || !ticketId) return;

        connection.invoke('joinTicketGroup', Number(ticketId));

        function handleComment(comment: Comment) {
            if (comment.authorId != currentUser.id) {
                if (currentUser.role === UserRole.Requester){
                    if (!comment.isInternal) setComments(prev => [...prev, comment]);
                } else {
                    setComments(prev => [...prev, comment]);
                }
            }
        }
        connection.on('RecieveComment', handleComment);

        return () => {
            connection.off('RecieveComment', handleComment);
            connection.invoke('leaveTicketGroup', Number(ticketId));
        }
    }, [connection, ticketId, currentUser]);

    useEffect(() => {
        if (updatedTickets?.id === Number(ticketId)){
            dispatch(fetchTicketById(Number(ticketId)));
            fetchTicketActivity(Number(ticketId));
        }
    }, [updatedTickets, ticketId])

    async function fetchTicketActivity(ticketId: number) {
        setActivityLoading(true);
        try {
            const response = await getAuditLogsByTicketIdAsync(ticketId);
            setActivities(response.data);
        } catch (error) {
            toast.error(error);
        } finally {
            setActivityLoading(false);
        }
    }

    async function fetchTicketComments(ticketId: number) {
        setCommentLoading(true);
        try {
            const response = await getCommentsByTicketIdAsync(ticketId);
            setComments(response.data);
        } catch (error) {
            toast.error(error);
        } finally {
            setCommentLoading(false);
        }
    }

    const handleAddComment = useCallback(
        async (content: string, isInternal: boolean) => {
            if (!ticketId) return;
            try {
                const response = await createCommentAsync({ ticketId: Number(ticketId), body: content, isInternal } as CreateCommentPayload);
                toast.success(response.message);
                setComments(prev => [...prev, response.data]);
            } catch (error) {
                toast.error(error);
            }
        },
        [ticketId]
    );

    const canPostInternal = currentUser?.role !== UserRole.Requester;

    return (
        <>
            <Stack direction='row' sx={{ justifyContent: 'space-between' }}>
                <Button variant='text' onClick={() => navigate(-1)}>Go Back</Button>
                {ticket && (
                    <TicketActionsBar
                        ticketId={ticket.id}
                        status={ticket.status}
                        assignedAgentId={ticket.assignedToId}
                        assignedAgentName={ticket.assignedToName}
                        role={currentUser?.role as UserRole}
                        onActionComplete={() => {
                            dispatch(fetchTicketById(Number(ticketId)));
                            fetchTicketActivity(Number(ticketId));
                        }}
                    />
                )}
            </Stack>
            {ticketLoading && (
                <Stack sx={{ py: 8, justifyContent: 'center', alignItems: 'center' }}>
                    <CircularProgress />
                </Stack>
            )}
            {!ticketLoading && ticketError && <Alert severity="error">{ticketError ?? 'Failed to load ticket'}</Alert>}
            {!ticketLoading && !ticketError && !ticket && <Typography>Ticket not found.</Typography>}
            {ticket && !ticketLoading && !ticketError && (
                <Box sx={{ py: 1 }}>
                    <Grid container spacing={3}>
                        <Grid size={{ xs: 12, md: 8 }}>
                            <Stack spacing={3}>
                                <TicketInfoCard ticket={ticket} />
                                {commentLoading ? (
                                    <CircularProgress size={24} />
                                ) : (
                                    <CommentsSection
                                        comments={comments}
                                        onAddComment={handleAddComment}
                                        canPostInternal={canPostInternal}
                                    />
                                )}
                            </Stack>
                        </Grid>
                        <Grid size={{ xs: 12, md: 4 }}>
                            {activityLoading ? (
                                <CircularProgress size={24} />
                            ) : (
                                <ActivityTimeline activities={activities} />
                            )}
                        </Grid>
                    </Grid>
                </Box>
            )
            }
        </>
    );
};

export default TicketDetailsPage;