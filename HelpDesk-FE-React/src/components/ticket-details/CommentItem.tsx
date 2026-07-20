import { alpha, Avatar, Chip, Paper, Stack, Typography } from "@mui/material";
import type { Comment } from "../../interfaces/comment";
import dayjs from "dayjs";

interface Props {
  comment: Comment;
}

const CommentItem = ({ comment }: Props) => {
  return (
    <Stack direction="row" spacing={1.5} sx={{ alignItems: "flex-start" }}>
      <Avatar sx={{ width: 32, height: 32, fontSize: 14 }}>
        {comment.authorName.charAt(0).toUpperCase()}
      </Avatar>
      <Paper
        variant="outlined"
        sx={(theme) => ({
          p: 1.5,
          flex: 1,
          bgcolor: comment.isInternal ? alpha(theme.palette.warning.main, 0.05) : 'background.paper',
          borderColor: comment.isInternal ? 'warning.light' : undefined,
        })}
      >
        <Stack direction="row" sx={{ justifyContent: "space-between", alignItems: "center" }}>
          <Typography variant="subtitle2">{comment.authorName}</Typography>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            {comment.isInternal && <Chip label="Internal" size="small" color="warning" />}
            <Typography variant="caption" color="text.secondary">
              {dayjs(comment.createdAt).format('DD MMM, hh:mm A')}
            </Typography>
          </Stack>
        </Stack>
        <Typography variant="body2" sx={{ mt: 0.5, whiteSpace: 'pre-wrap' }}>
          {comment.body}
        </Typography>
      </Paper >
    </Stack >
  );
};

export default CommentItem;