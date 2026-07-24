import { useEffect, useState, type MouseEvent, type UIEvent } from "react";
import { useAppDispatch, useAppSelector } from "../../app/hooks/hooks";
import { useNavigate } from "react-router-dom";
import { fetchNotifications, fetchUnReadCount, markAllNotificationRead, markNotificationRead } from "../../app/thunks/notification-thunk";
import type { NotificationResponse } from "../../interfaces/notification";
import { toast } from "../../utils/Toast";
import { Badge, Button, CircularProgress, Divider, IconButton, List, ListItem, ListItemButton, ListItemText, Menu, Stack, Tooltip, Typography } from "@mui/material";
import { FiberManualRecord, Notifications } from "@mui/icons-material";
import dayjs from "dayjs";

const PAGE_SIZE = 10;
const SCROLL_THRESHOLD = 32;

function NotificationBell() {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const { notifications, unreadCount, loading, page, totalPages } = useAppSelector((state) => state.notification);

    useEffect(() => {
        dispatch(fetchUnReadCount());
    }, [dispatch]);

    function handleOpen(event: MouseEvent<HTMLElement>) {
        setAnchorEl(event.currentTarget);
        dispatch(fetchNotifications({ page: 1, pageSize: 10 }));
    }

    function handleClose() {
        setAnchorEl(null);
    }

    function handleScroll(event: UIEvent<HTMLUListElement>) {
        const target = event.currentTarget;
        const reachedBottom = target.scrollTop + target.clientHeight >= target.scrollHeight - SCROLL_THRESHOLD;

        if (reachedBottom && !loading && page < totalPages) {
            dispatch(fetchNotifications({ page: page + 1, pageSize: PAGE_SIZE }));
        }
    }

    // Click on the notification body: mark read AND open its ticket.
    async function handleNotificationClick(notification: NotificationResponse) {
        try {
            await dispatch(markNotificationRead(notification.id)).unwrap();
        } catch (err) {
            toast.error(err);
        }
        handleClose();
        if (notification.ticketId) {
            navigate(`/ticket/${notification.ticketId}`);
        }
    }

    // Click on the dot: mark read only, stay on the dropdown, don't navigate.
    async function handleMarkReadOnly(id: number) {
        try {
            await dispatch(markNotificationRead(id)).unwrap();
        } catch (err) {
            toast.error(err);
        }
    }

    async function handleMarkAllRead() {
        try {
            await dispatch(markAllNotificationRead()).unwrap();
        } catch (err) {
            toast.error(err);
        }
    }

    const isInitialLoad = loading && notifications.length === 0;
    const isLoadingMore = loading && notifications.length > 0;

    return (
        <>
            <IconButton color="inherit" onClick={handleOpen}>
                <Badge badgeContent={unreadCount} color="error" max={99}>
                    <Notifications />
                </Badge>
            </IconButton>

            <Menu
                anchorEl={anchorEl}
                open={!!anchorEl}
                onClose={handleClose}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                transformOrigin={{ vertical: 'top', horizontal: 'right' }}
                slotProps={{ paper: { sx: { width: 360, maxHeight: 480 } } }}
            >
                <Stack direction="row" sx={{ px: 2, py: 1, alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>Notifications</Typography>
                    <Button size="small" onClick={handleMarkAllRead} disabled={unreadCount === 0}>
                        Mark all read
                    </Button>
                </Stack>
                <Divider />

                {isInitialLoad ? (
                    <Stack sx={{ py: 3, alignItems: 'center' }}>
                        <CircularProgress size={24} />
                    </Stack>
                ) : notifications.length === 0 ? (
                    <Typography variant="body2" color="text.secondary" sx={{ p: 2 }}>
                        No notifications yet.
                    </Typography>
                ) : (
                    <List disablePadding onScroll={handleScroll} sx={{ maxHeight: 380, overflowY: 'auto' }}>
                        {notifications.map((notification) => (
                            <ListItem
                                key={notification.id}
                                disablePadding
                                secondaryAction={
                                    <Tooltip title="Mark as read">
                                        <IconButton
                                            size="small"
                                            edge="end"
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                handleMarkReadOnly(notification.id);
                                            }}
                                        >
                                            <FiberManualRecord color="primary" sx={{ fontSize: 12 }} />
                                        </IconButton>
                                    </Tooltip>
                                }
                            >
                                <ListItemButton onClick={() => handleNotificationClick(notification)} sx={{ pr: 5 }}>
                                    <ListItemText
                                        primary={notification.message}
                                        secondary={dayjs(notification.createdAt).format('DD MMM YYYY, hh:mm A')}
                                    />
                                </ListItemButton>
                            </ListItem>
                        ))}
                        {isLoadingMore && (
                            <Stack sx={{ py: 1.5, alignItems: 'center' }}>
                                <CircularProgress size={18} />
                            </Stack>
                        )}
                    </List>
                )}
            </Menu>
        </>
    );
}

export default NotificationBell;