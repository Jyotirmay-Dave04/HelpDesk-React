import { Box, Card, CardContent, Chip, Divider, Grid, Stack, Typography } from "@mui/material";
import type { TicketResponse } from "../../interfaces/ticket";
import { getStatusColor, getPriorityColor, formatDuration } from "../../utils/TicketDisplay";
import dayjs from 'dayjs';
import { TicketStatus } from "../../types/ticket-status";

interface Props {
    ticket: TicketResponse;
}

const TicketInfoCard = ({ ticket }: Props) => {
    return (
        <Card variant="outlined">
            <CardContent>
                <Stack direction="row" spacing={1} sx={{justifyConten:"space-between", alignItems:"flex-start"}}>
                    <Box>
                        <Typography variant='subtitle1' color="text.secondary">
                            #{ticket.id}
                        </Typography>
                    </Box>
                    <Stack direction="row" spacing={1}>
                        <Chip label={ticket.status} color={getStatusColor(ticket.status)} size="small" />
                        <Chip label={ticket.priority} color={getPriorityColor(ticket.priority)} size="small" variant="outlined" />
                    </Stack>
                </Stack>

                <Typography variant="body1" sx={{ mt: 2, whiteSpace: 'pre-wrap' }}>
                    {ticket.serviceDetails}
                </Typography>

                <Divider sx={{ my: 2 }} />

                <Grid container spacing={2}>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Group</Typography>
                        <Typography variant="body2">{ticket.groupName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Category</Typography>
                        <Typography variant="body2">{ticket.categoryName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Sub-Category</Typography>
                        <Typography variant="body2">{ticket.subCategoryName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Requester</Typography>
                        <Typography variant="body2">{ticket.requesterName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Assigned To</Typography>
                        <Typography variant="body2">{ticket.assignedToName ?? 'Unassigned'}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">{ticket.status === TicketStatus.Resolved || ticket.status === TicketStatus.Closed ? 'Resolved At' : 'SLA Deadline'}</Typography>
                        <Typography variant="body2">{
                            ticket.status === TicketStatus.Resolved || ticket.status === TicketStatus.Closed
                                ? dayjs(ticket.resolvedAt).format('DD MMM YYYY, hh:mm A')
                                : dayjs(ticket.slaDeadline).format('DD MMM YYYY, hh:mm A')
                        }</Typography>
                    </Grid>
                    {ticket && ticket.onHoldPausedSeconds > 0 && (
                        <Grid size={{ xs: 6, sm: 3 }}>
                            <Typography variant="caption" color="text.secondary">Total Time Paused</Typography>
                            <Typography variant="body2">{formatDuration(ticket.onHoldPausedSeconds)}</Typography>
                        </Grid>
                    )}
                    {ticket && ticket.reopenCount > 0 && (
                        <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Reopened</Typography>
                        <Typography variant="body2">{ticket.reopenCount} {ticket.reopenCount === 1 ? 'time' : 'times'}</Typography>
                    </Grid>
                    )}
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Created On</Typography>
                        <Typography variant="body2">{dayjs(ticket.createdAt).format('DD MMM YYYY, hh:mm A')}</Typography>
                    </Grid>
                    <Grid size={{ xs: 6, sm: 3 }}>
                        <Typography variant="caption" color="text.secondary">Last Updated On</Typography>
                        <Typography variant="body2">{dayjs(ticket.modifiedAt).format('DD MMM YYYY, hh:mm A')}</Typography>
                    </Grid>
                </Grid>
            </CardContent>
        </Card>
    );
}

export default TicketInfoCard;