import React from "react";
import { LinearProgress, Box, Typography } from "@mui/material";

interface ProgressBarProps {
  planned: number;
  produced: number;
}

const ProgressBar: React.FC<ProgressBarProps> = ({ planned, produced }) => {
  const percentage =
    planned > 0 ? Math.min((produced / planned) * 100, 100) : 0;

  return (
    <Box display="flex" alignItems="center" gap={1}>
      <Box width="100%">
        <LinearProgress
          variant="determinate"
          value={percentage}
          color={percentage >= 100 ? "success" : "primary"}
        />
      </Box>
      <Box minWidth={35}>
        <Typography variant="body2" color="textSecondary">
          {`${Math.round(percentage)}%`}
        </Typography>
      </Box>
    </Box>
  );
};

export default ProgressBar;
