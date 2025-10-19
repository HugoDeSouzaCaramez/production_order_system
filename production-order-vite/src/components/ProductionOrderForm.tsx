import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  MenuItem,
  Box,
  InputAdornment,
} from "@mui/material";
import { productionOrderService, productService } from "../services/api";
import type {
  CreateProductionOrderDto,
  Product,
} from "../types/productionOrder";

interface ProductionOrderFormProps {
  open: boolean;
  onClose: () => void;
  onOrderCreated: () => void;
  showSnackbar: (message: string, severity?: "success" | "error") => void;
}

const ProductionOrderForm: React.FC<ProductionOrderFormProps> = ({
  open,
  onClose,
  onOrderCreated,
  showSnackbar,
}) => {
  const [formData, setFormData] = useState<CreateProductionOrderDto>({
    orderNumber: "",
    productCode: "",
    quantityPlanned: 0,
    status: 1,
    startDate: new Date().toISOString().split("T")[0],
  });

  const [products, setProducts] = useState<Product[]>([]);
  const [statusOptions, setStatusOptions] = useState<
    { value: number; label: string }[]
  >([]);
  const [loading, setLoading] = useState(false);
  const [productsLoading, setProductsLoading] = useState(false);
  const [statusLoading, setStatusLoading] = useState(false);

  useEffect(() => {
    if (open) {
      fetchProducts();
      fetchStatusOptions();
    }
  }, [open]);

  const fetchProducts = async () => {
    setProductsLoading(true);
    try {
      const productsData = await productService.getProducts();
      setProducts(productsData);
    } catch (error) {
      console.error("Erro ao carregar produtos:", error);
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Erro ao carregar lista de produtos";
      showSnackbar(errorMessage, "error");
    } finally {
      setProductsLoading(false);
    }
  };

  const fetchStatusOptions = async () => {
    setStatusLoading(true);
    try {
      const statuses = await productionOrderService.getPossibleStatuses();
      const statusOptions = statuses.map((status) => {
        switch (status) {
          case "Planejada":
            return { value: 1, label: "Planejada" };
          case "EmProducao":
            return { value: 2, label: "Em Produção" };
          case "Finalizada":
            return { value: 3, label: "Finalizada" };
          default:
            return { value: 1, label: status };
        }
      });
      setStatusOptions(statusOptions);

      if (statusOptions.length > 0 && !formData.status) {
        setFormData((prev) => ({ ...prev, status: statusOptions[0].value }));
      }
    } catch (error) {
      console.error("Erro ao carregar status:", error);
      const errorMessage =
        error instanceof Error ? error.message : "Erro ao carregar status";
      showSnackbar(errorMessage, "error");
      setStatusOptions([
        { value: 1, label: "Planejada" },
        { value: 2, label: "Em Produção" },
        { value: 3, label: "Finalizada" },
      ]);
    } finally {
      setStatusLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const orderData = {
        ...formData,
        orderNumber: `ORD-${formData.orderNumber}`,
      };

      await productionOrderService.createOrder(orderData);
      showSnackbar("Ordem criada com sucesso!");
      onOrderCreated();
      onClose();
      resetForm();
    } catch (error) {
      console.error("Erro ao criar ordem:", error);
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Erro ao criar ordem de produção";
      showSnackbar(errorMessage, "error");
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setFormData({
      orderNumber: "",
      productCode: "",
      quantityPlanned: 0,
      status: 1,
      startDate: new Date().toISOString().split("T")[0],
    });
  };

  const handleClose = () => {
    onClose();
    resetForm();
  };

  const getDisplayOrderNumber = (orderNumber: string) => {
    return orderNumber.replace(/^ORD-/, "");
  };

  const productOptions = products.map((product) => ({
    value: product.code,
    label: `${product.code} - ${product.description}`,
  }));

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Nova Ordem de Produção</DialogTitle>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Número da Ordem"
              value={getDisplayOrderNumber(formData.orderNumber)}
              onChange={(e) => {
                const suffix = e.target.value.replace(/[^0-9]/g, "");
                setFormData({ ...formData, orderNumber: suffix });
              }}
              required
              fullWidth
              disabled={loading}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">ORD-</InputAdornment>
                ),
              }}
              placeholder="12345"
              helperText="Digite apenas o número da ordem (sufixo)"
            />

            <TextField
              select
              label="Produto"
              value={formData.productCode}
              onChange={(e) =>
                setFormData({ ...formData, productCode: e.target.value })
              }
              required
              fullWidth
              disabled={loading || productsLoading}
            >
              {productsLoading ? (
                <MenuItem value="">Carregando produtos...</MenuItem>
              ) : (
                productOptions.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))
              )}
            </TextField>

            <TextField
              label="Quantidade Planejada"
              type="number"
              value={
                formData.quantityPlanned === 0 ? "" : formData.quantityPlanned
              }
              onChange={(e) => {
                const value = e.target.value;
                const quantity = value === "" ? 0 : parseInt(value) || 0;
                setFormData({
                  ...formData,
                  quantityPlanned: quantity,
                });
              }}
              required
              fullWidth
              inputProps={{ min: 1 }}
              disabled={loading}
            />

            <TextField
              select
              label="Status"
              value={formData.status}
              onChange={(e) =>
                setFormData({ ...formData, status: parseInt(e.target.value) })
              }
              required
              fullWidth
              disabled={loading || statusLoading}
            >
              {statusLoading ? (
                <MenuItem value="">Carregando status...</MenuItem>
              ) : (
                statusOptions.map((status) => (
                  <MenuItem key={status.value} value={status.value}>
                    {status.label}
                  </MenuItem>
                ))
              )}
            </TextField>

            <TextField
              label="Data de Início"
              type="date"
              value={formData.startDate}
              onChange={(e) =>
                setFormData({ ...formData, startDate: e.target.value })
              }
              required
              fullWidth
              InputLabelProps={{ shrink: true }}
              disabled={loading}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} disabled={loading}>
            Cancelar
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={
              loading ||
              productsLoading ||
              statusLoading ||
              !formData.orderNumber
            }
          >
            {loading ? "Criando..." : "Criar Ordem"}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default ProductionOrderForm;
