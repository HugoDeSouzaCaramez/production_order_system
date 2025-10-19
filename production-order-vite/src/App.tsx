import React, { useState, useEffect } from "react";
import {
  Container,
  Typography,
  Button,
  Box,
  AppBar,
  Toolbar,
  Snackbar,
  Alert,
} from "@mui/material";
import { Add } from "@mui/icons-material";
import ProductionOrderForm from "./components/ProductionOrderForm";
import ProductionOrderEditForm from "./components/ProductionOrderEditForm";
import ProductionOrderList from "./components/ProductionOrderList";
import { productionOrderService } from "./services/api";
import type { ProductionOrder } from "./types/productionOrder";

const App: React.FC = () => {
  const [orders, setOrders] = useState<ProductionOrder[]>([]);
  const [initialLoading, setInitialLoading] = useState(true); // Renomeado
  const [formOpen, setFormOpen] = useState(false);
  const [editFormOpen, setEditFormOpen] = useState(false);
  const [editingOrderId, setEditingOrderId] = useState<number | null>(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });
  const [updatingOrders, setUpdatingOrders] = useState<Set<number>>(new Set()); // Estado para ordens sendo atualizadas

  const showSnackbar = (
    message: string,
    severity: "success" | "error" = "success"
  ) => {
    setSnackbar({ open: true, message, severity });
  };

  const fetchOrders = async () => {
    try {
      const data = await productionOrderService.getOrders();
      setOrders(data);
    } catch (error: any) {
      console.error("Erro ao buscar ordens:", error);
      showSnackbar(error.message, "error");
    } finally {
      setInitialLoading(false);
    }
  };

  const handleStatusUpdate = async (orderId: number, newStatus: string) => {
    try {
      setUpdatingOrders((prev) => new Set(prev).add(orderId));

      let statusNumber: number;
      switch (newStatus) {
        case "Planejada":
          statusNumber = 1;
          break;
        case "Em Produção":
          statusNumber = 2;
          break;
        case "Finalizada":
          statusNumber = 3;
          break;
        default:
          statusNumber = 1;
      }

      await productionOrderService.updateOrderStatus(orderId, statusNumber);

      setOrders((prevOrders) =>
        prevOrders.map((order) =>
          order.id === orderId ? { ...order, status: statusNumber } : order
        )
      );

      showSnackbar("Status atualizado com sucesso!");
    } catch (error: any) {
      console.error("Erro ao atualizar status:", error);
      showSnackbar(error.message, "error");
    } finally {
      setUpdatingOrders((prev) => {
        const newSet = new Set(prev);
        newSet.delete(orderId);
        return newSet;
      });
    }
  };

  const handleEditOrder = (orderId: number) => {
    setEditingOrderId(orderId);
    setEditFormOpen(true);
  };

  const handleOrderUpdated = () => {
    fetchOrders();
  };

  const handleOrderCreated = () => {
    fetchOrders();
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Sistema de Ordens de Produção
          </Typography>
        </Toolbar>
      </AppBar>

      <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
        <Box
          display="flex"
          justifyContent="space-between"
          alignItems="center"
          mb={3}
        >
          <Typography variant="h4" component="h1">
            Ordens de Produção
          </Typography>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setFormOpen(true)}
          >
            Nova Ordem
          </Button>
        </Box>

        <ProductionOrderList
          orders={orders}
          onStatusUpdate={handleStatusUpdate}
          onEditOrder={handleEditOrder}
          loading={initialLoading}
          updatingOrders={updatingOrders} // Nova prop
        />

        <ProductionOrderForm
          open={formOpen}
          onClose={() => setFormOpen(false)}
          onOrderCreated={handleOrderCreated}
          showSnackbar={showSnackbar}
        />

        <ProductionOrderEditForm
          open={editFormOpen}
          onClose={() => setEditFormOpen(false)}
          onOrderUpdated={handleOrderUpdated}
          orderId={editingOrderId}
          showSnackbar={showSnackbar}
        />

        <Snackbar
          open={snackbar.open}
          autoHideDuration={6000}
          onClose={() => setSnackbar({ ...snackbar, open: false })}
        >
          <Alert
            onClose={() => setSnackbar({ ...snackbar, open: false })}
            severity={snackbar.severity}
          >
            {snackbar.message}
          </Alert>
        </Snackbar>
      </Container>
    </>
  );
};

export default App;
