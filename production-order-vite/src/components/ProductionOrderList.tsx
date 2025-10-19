import React, { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Chip,
  Box,
  Typography,
  TextField,
  InputAdornment,
  TablePagination,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  CircularProgress,
} from "@mui/material";
import { PlayArrow, Check, Refresh, Edit, Search } from "@mui/icons-material";
import type { ProductionOrder } from "../types/productionOrder";
import ProgressBar from "./ProgressBar";

interface ProductionOrderListProps {
  orders: ProductionOrder[];
  onStatusUpdate: (orderId: number, newStatus: string) => void;
  onEditOrder: (orderId: number) => void;
  loading?: boolean;
  updatingOrders?: Set<number>;
}

const ProductionOrderList: React.FC<ProductionOrderListProps> = ({
  orders,
  onStatusUpdate,
  onEditOrder,
  loading = false,
  updatingOrders = new Set(),
}) => {
  const [searchTerm, setSearchTerm] = useState("");
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const rowsPerPageOptions = [5, 10, 20, 50, 100, 500, 1000];

  const getStatusLabel = (status: number): string => {
    const statusMap: { [key: number]: string } = {
      1: "Planejada",
      2: "Em Produção",
      3: "Finalizada",
    };
    return statusMap[status] || "Desconhecido";
  };

  const getNextStatusNumber = (currentStatus: number): number => {
    const statusTransitions: { [key: number]: number } = {
      1: 2,
      2: 3,
      3: 1,
    };
    return statusTransitions[currentStatus] || 1;
  };

  const getNextStatusLabel = (currentStatus: number): string => {
    const nextStatusNumber = getNextStatusNumber(currentStatus);
    return getStatusLabel(nextStatusNumber);
  };

  const getStatusColor = (status: number) => {
    switch (status) {
      case 1:
        return "default";
      case 2:
        return "primary";
      case 3:
        return "success";
      default:
        return "default";
    }
  };

  const getStatusIcon = (status: number) => {
    switch (status) {
      case 1:
        return <PlayArrow />;
      case 2:
        return <Check />;
      case 3:
        return <Refresh />;
      default:
        return <PlayArrow />;
    }
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString("pt-BR");
  };

  const filteredOrders = orders.filter((order) =>
    order.orderNumber.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const paginatedOrders = filteredOrders.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  React.useEffect(() => {
    setPage(0);
  }, [searchTerm]);

  const isOrderUpdating = (orderId: number) => {
    return updatingOrders.has(orderId);
  };

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        height={200}
      >
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>Carregando ordens...</Typography>
      </Box>
    );
  }

  return (
    <Box>
      {/* Cabeçalho com busca e controles de paginação */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
        gap={2}
      >
        <Box sx={{ minWidth: 300, flex: 1 }}>
          <TextField
            fullWidth
            variant="outlined"
            placeholder="Buscar por número da ordem..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search color="action" />
                </InputAdornment>
              ),
            }}
            size="small"
          />
        </Box>

        {/* Seletor de itens por página */}
        <FormControl size="small" sx={{ minWidth: 120 }}>
          <InputLabel>Itens por página</InputLabel>
          <Select
            value={rowsPerPage}
            label="Itens por página"
            onChange={(e) => setRowsPerPage(Number(e.target.value))}
          >
            {rowsPerPageOptions.map((option) => (
              <MenuItem key={option} value={option}>
                {option}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>

      {/* Informações de exibição atual */}
      <Box mb={1}>
        <Typography variant="body2" color="textSecondary">
          Exibindo {paginatedOrders.length} de {filteredOrders.length} ordens
          {searchTerm && ` (filtradas de ${orders.length} totais)`}
        </Typography>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Ordem</TableCell>
              <TableCell>Produto</TableCell>
              <TableCell>Quantidade Planejada</TableCell>
              <TableCell>Quantidade Produzida</TableCell>
              <TableCell>Progresso</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Data de Início</TableCell>
              <TableCell>Data de Finalização</TableCell>
              <TableCell>Ações</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedOrders.map((order) => {
              const isUpdating = isOrderUpdating(order.id);

              return (
                <TableRow key={order.id}>
                  <TableCell>{order.orderNumber}</TableCell>
                  <TableCell>
                    <Box>
                      <Typography variant="body2">
                        {order.productCode}
                      </Typography>
                      {order.product && (
                        <Typography variant="caption" color="textSecondary">
                          {order.product.description}
                        </Typography>
                      )}
                    </Box>
                  </TableCell>
                  <TableCell>
                    {order.quantityPlanned.toLocaleString()}
                  </TableCell>
                  <TableCell>
                    {order.quantityProduced.toLocaleString()}
                  </TableCell>
                  <TableCell>
                    <ProgressBar
                      planned={order.quantityPlanned}
                      produced={order.quantityProduced}
                    />
                  </TableCell>
                  <TableCell>
                    {isUpdating ? (
                      <Box display="flex" alignItems="center" gap={1}>
                        <CircularProgress size={20} />
                        <Typography variant="caption">
                          Atualizando...
                        </Typography>
                      </Box>
                    ) : (
                      <Chip
                        label={getStatusLabel(order.status)}
                        color={getStatusColor(order.status) as any}
                        size="small"
                      />
                    )}
                  </TableCell>
                  <TableCell>{formatDate(order.startDate)}</TableCell>
                  <TableCell>
                    {order.endDate ? formatDate(order.endDate) : ""}
                  </TableCell>
                  <TableCell>
                    <Box display="flex" gap={1}>
                      <IconButton
                        onClick={() =>
                          !isUpdating &&
                          onStatusUpdate(
                            order.id,
                            getNextStatusLabel(order.status)
                          )
                        }
                        color="primary"
                        title={`Atualizar status para ${getNextStatusLabel(
                          order.status
                        )}`}
                        size="small"
                        disabled={isUpdating}
                      >
                        {isUpdating ? (
                          <CircularProgress size={20} />
                        ) : (
                          getStatusIcon(order.status)
                        )}
                      </IconButton>
                      <IconButton
                        onClick={() => onEditOrder(order.id)}
                        color="secondary"
                        title="Editar ordem"
                        size="small"
                        disabled={isUpdating}
                      >
                        <Edit />
                      </IconButton>
                    </Box>
                  </TableCell>
                </TableRow>
              );
            })}
            {paginatedOrders.length === 0 && (
              <TableRow>
                <TableCell colSpan={9} align="center">
                  <Typography variant="body2" color="textSecondary">
                    {searchTerm
                      ? "Nenhuma ordem encontrada para a busca"
                      : "Nenhuma ordem cadastrada"}
                  </Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Paginação */}
      <TablePagination
        rowsPerPageOptions={rowsPerPageOptions}
        component="div"
        count={filteredOrders.length}
        rowsPerPage={rowsPerPage}
        page={page}
        onPageChange={handleChangePage}
        onRowsPerPageChange={handleChangeRowsPerPage}
        labelRowsPerPage="Itens por página:"
        labelDisplayedRows={({ from, to, count }) =>
          `${from}-${to} de ${count !== -1 ? count : `mais de ${to}`}`
        }
        sx={{
          ".MuiTablePagination-toolbar": {
            padding: 1,
          },
        }}
      />
    </Box>
  );
};

export default ProductionOrderList;
