import { Card, CardContent, Typography } from "@mui/material";
import type { AuditLog } from "../../interfaces/audit-log";
import { AuditAction } from "../../types/audit-action";
import { Timeline, TimelineConnector, TimelineContent, TimelineDot, TimelineItem, TimelineOppositeContent, TimelineSeparator } from '@mui/lab';
import dayjs from "dayjs";

interface Props {
    activities: AuditLog[];
}

const describeActivity = (log: AuditLog): string => {
    const action = log.action as AuditAction;
    if (action === AuditAction.ASSIGNED) {
        return `Assigned to ${log.newValue ?? '—'}`;
    }
    if (action === AuditAction.STATUS_CHANGED) {
        return `Status changed from ${log.oldValue ?? '—'} to ${log.newValue ?? '—'}`;
    }
    if (action === AuditAction.CREATED) {
        return 'Ticket created';
    }
    if (action === AuditAction.UPDATED) {
        const field = log.fieldName ?? 'field';
        return `Updated ${field}`;
    }
    if (action === AuditAction.SLA_BREACHED) {
        return 'SLA deadline breached';
    }
    return log.action;
};

const ActivityTimeline = ({ activities }: Props) => {
    if (activities.length === 0) {
        return (
            <Card variant="outlined">
                <CardContent>
                    <Typography variant="body2" color="text.secondary">
                        No activity yet.
                    </Typography>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card variant="outlined">
            <CardContent>
                <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>
                    Activity Timeline
                </Typography>
                <Timeline sx={{ p: 0, m: 0 }}>
                    {activities.map((log, index) => (
                        <TimelineItem key={log.id} sx={{ minHeight: 70 }}>
                            <TimelineOppositeContent sx={{ flex: 0.3, px: 1 }}>
                                <Typography variant="caption" color="text.secondary">
                                    {dayjs(log.createdAt).format('DD MMM, hh:mm A')}
                                </Typography>
                            </TimelineOppositeContent>
                            <TimelineSeparator>
                                <TimelineDot color="primary" variant="outlined" />
                                {index < activities.length - 1 && <TimelineConnector />}
                            </TimelineSeparator>
                            <TimelineContent sx={{ px: 2 }}>
                                <Typography variant="body2">{describeActivity(log)}</Typography>
                                <Typography variant="caption" color="text.secondary">
                                    by {log.changedByName}
                                </Typography>
                                {log.reason && (
                                    <Typography variant="caption" sx={{ display: "block", fontStyle: "italic" }}>
                                        Reason: {log.reason}
                                    </Typography>
                                )}
                            </TimelineContent>
                        </TimelineItem>
                    ))}
                </Timeline>
            </CardContent>
        </Card>
    );
};

export default ActivityTimeline;