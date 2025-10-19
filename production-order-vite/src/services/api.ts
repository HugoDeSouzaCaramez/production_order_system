import axios, { AxiosError } from "axios";
import type {
  CreateProductionOrderDto,
  Product,
  ProductionOrder,
  UpdateProductionOrderDto,
} from "../types/productionOrder";

const API_BASE_URL = "http://localhost:5079/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

const getErrorMessage = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<{ message?: string }>;
    if (axiosError.response?.data?.message) {
      return axiosError.response.data.message;
    }
  }

  return "Erro desconhecido ao conectar com o servidor";
};

export const productionOrderService = {
  getOrders: async (): Promise<ProductionOrder[]> => {
    try {
      const response = await api.get<ProductionOrder[]>("/orders");
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  getOrderById: async (id: number): Promise<ProductionOrder> => {
    try {
      const response = await api.get<ProductionOrder>(`/orders/${id}`);
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  getOrdersByStatus: async (status: string): Promise<ProductionOrder[]> => {
    try {
      const response = await api.get<ProductionOrder[]>(
        `/orders/status/${status}`
      );
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  getPossibleStatuses: async (): Promise<string[]> => {
    try {
      const response = await api.get<string[]>("/orders/status/possible");
      console.log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
      console.log("response: ", response);
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  createOrder: async (
    orderData: CreateProductionOrderDto
  ): Promise<ProductionOrder> => {
    try {
      const response = await api.post<ProductionOrder>("/orders", orderData);
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  updateOrder: async (
    id: number,
    updateData: UpdateProductionOrderDto
  ): Promise<ProductionOrder> => {
    try {
      const response = await api.patch<ProductionOrder>(
        `/orders/${id}`,
        updateData
      );
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },

  updateOrderStatus: async (
    id: number,
    status: number
  ): Promise<ProductionOrder> => {
    try {
      const response = await api.patch<ProductionOrder>(`/orders/${id}`, {
        status,
      });
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },
};

export const productService = {
  getProducts: async (): Promise<Product[]> => {
    try {
      const response = await api.get<Product[]>("/products");
      return response.data;
    } catch (error) {
      const errorMessage = getErrorMessage(error);
      throw new Error(errorMessage);
    }
  },
};
