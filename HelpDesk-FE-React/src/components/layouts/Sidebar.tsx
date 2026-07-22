import { useLocation, useNavigate } from "react-router-dom";
import { useAppSelector } from "../../app/hooks/hooks";
import { Box, Drawer, List, ListItemButton, ListItemIcon, ListItemText, Toolbar, Tooltip } from "@mui/material";
import { NAV_ITEMS } from "../../constants/nav-items";
import type { UserRole } from "../../types/user-role";

export const SIDEBAR_WIDTH_FULL = 240;
export const SIDEBAR_WIDTH_MINI = 64;

interface SidebarProps {
    variant: 'temporary' | 'permanent';
    open: boolean;
    collapsed: boolean;
    onClose: () => void;
}

export function Sidebar({ variant, open, collapsed, onClose }: SidebarProps) {
    const navigate = useNavigate();
    const location = useLocation();
    const { user } = useAppSelector(state => state.auth);

    const visibleItems = NAV_ITEMS.filter((items) => user && items.roles.includes(user.role as UserRole));
    
    const width = variant === 'temporary' ? SIDEBAR_WIDTH_FULL : (collapsed ? SIDEBAR_WIDTH_MINI : SIDEBAR_WIDTH_FULL);

    return (
        <Drawer
            variant={variant}
            open={variant === "temporary" ? open : true}
            onClose={onClose}
            ModalProps={{ keepMounted: true }}
            sx={{
                width,
                flexShrink: 0,
                whiteSpace: "nowrap",
                "& .MuiDrawer-paper": {
                    width,
                    overflowX: "hidden",
                    transition: (theme) =>
                        theme.transitions.create("width", {
                            easing: theme.transitions.easing.sharp,
                            duration: theme.transitions.duration.enteringScreen,
                        }),
                    boxSizing: "border-box",
                },
            }}
        >
            <Toolbar />
            <Box sx={{ overflow: "auto" }}>
                <List>
                    {visibleItems.map((item) => {
                        const Icon = item.icon;
                        const isActive = location.pathname.startsWith(item.path);
                        const button = (
                            <ListItemButton
                                key={item.path}
                                selected={isActive}
                                onClick={() => {
                                    navigate(item.path);
                                    if (variant === "temporary") onClose();
                                }}
                                sx={{
                                    justifyContent: collapsed && variant === "permanent" ? "center" : "flex-start",
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon sx={{ minWidth: 0, mr: collapsed && variant === "permanent" ? 0 : 2, justifyContent: "center" }}>
                                    <Icon color={isActive ? "primary" : "inherit"} />
                                </ListItemIcon>
                                {(!collapsed || variant === "temporary") && <ListItemText primary={item.label} />}
                            </ListItemButton>
                        );

                        return collapsed && variant === "permanent" ? (
                            <Tooltip title={item.label} placement="right" key={item.path}>
                                {button}
                            </Tooltip>
                        ) : (
                            button
                        );
                    })}
                </List>
            </Box>
        </Drawer>
    );
}