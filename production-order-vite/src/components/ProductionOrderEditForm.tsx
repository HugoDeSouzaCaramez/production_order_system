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
  Typography,
  InputAdornment,
} from "@mui/material";
import { productionOrderService, productService } from "../services/api";
import type {
  ProductionOrder,
  UpdateProductionOrderDto,
  Product,
} from "../types/productionOrder";

interface ProductionOrderEditFormProps {
  open: boolean;
  onClose: () => void;
  onOrderUpdated: () => void;
  orderId: number | null;
  showSnackbar: (message: string, severity?: "success" | "error") => void;
}

const ProductionOrderEditForm: React.FC<ProductionOrderEditFormProps> = ({
  open,
  onClose,
  onOrderUpdated,
  orderId,
  showSnackbar,
}) => {
  const [formData, setFormData] = useState<UpdateProductionOrderDto>({
    orderNumber: "",
    productCode: "",
    quantityPlanned: 0,
    status: 1,
    startDate: new Date().toISOString().split("T")[0],
  });

  const [originalOrder, setOriginalOrder] = useState<ProductionOrder | null>(
    null
  );
  const [products, setProducts] = useState<Product[]>([]);
  const [statusOptions, setStatusOptions] = useState<
    { value: number; label: string }[]
  >([]);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(false);
  const [productsLoading, setProductsLoading] = useState(false);
  const [statusLoading, setStatusLoading] = useState(false);

  useEffect(() => {
    if (open && orderId) {
      fetchOrderData();
      fetchProducts();
      fetchStatusOptions();
    }
  }, [open, orderId]);

  const fetchOrderData = async () => {
    if (!orderId) return;

    setFetching(true);
    try {
      const order = await productionOrderService.getOrderById(orderId);
      setOriginalOrder(order);

      const orderNumberWithoutPrefix = order.orderNumber.replace(/^ORD-/, "");

      setFormData({
        orderNumber: orderNumberWithoutPrefix,
        productCode: order.productCode,
        quantityPlanned: order.quantityPlanned,
        status: order.status,
        startDate: order.startDate.split("T")[0],
      });
    } catch (error) {
      console.error("Erro ao carregar ordem:", error);
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Erro ao carregar dados da ordem";
      showSnackbar(errorMessage, "error");
    } finally {
      setFetching(false);
    }
  };

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
    if (!orderId) return;

    setLoading(true);

    try {
      const updateData: UpdateProductionOrderDto = {
        ...formData,
        orderNumber: `ORD-${formData.orderNumber}`,
        startDate: formData.startDate
          ? new Date(formData.startDate).toISOString()
          : undefined,
      };

      await productionOrderService.updateOrder(orderId, updateData);
      showSnackbar("Ordem atualizada com sucesso!");
      onOrderUpdated();
      onClose();
      resetForm();
    } catch (error) {
      console.error("Erro ao atualizar ordem:", error);
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Erro ao atualizar ordem de produção";
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
    setOriginalOrder(null);
    setProducts([]);
    setStatusOptions([]);
  };

  const handleClose = () => {
    onClose();
    resetForm();
  };

  const productOptions = products.map((product) => ({
    value: product.code,
    label: `${product.code} - ${product.description}`,
  }));

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>
          {fetching
            ? "Carregando..."
            : `Editar Ordem - ORD-${originalOrder?.orderNumber.replace(
                /^ORD-/,
                ""
              )}`}
        </DialogTitle>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Número da Ordem"
              value={formData.orderNumber}
              onChange={(e) => {
                const suffix = e.target.value.replace(/[^0-9]/g, "");
                setFormData({ ...formData, orderNumber: suffix });
              }}
              required
              fullWidth
              disabled={fetching}
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
              disabled={fetching || productsLoading}
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
              disabled={fetching}
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
              disabled={fetching || statusLoading}
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
              disabled={fetching}
            />

            {originalOrder && (
              <Box mt={1} p={1} bgcolor="grey.100" borderRadius={1}>
                <Typography variant="body2" color="textSecondary">
                  <strong>Quantidade Produzida:</strong>{" "}
                  {originalOrder.quantityProduced}
                  <br />
                  <em>
                    Este campo é atualizado automaticamente pelos logs de
                    produção
                  </em>
                </Typography>
              </Box>
            )}
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
              fetching ||
              productsLoading ||
              statusLoading ||
              !formData.orderNumber
            }
          >
            {loading ? "Atualizando..." : "Atualizar Ordem"}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default ProductionOrderEditForm;
