export interface ProductionOrder {
  id: number;
  orderNumber: string;
  productCode: string;
  quantityPlanned: number;
  quantityProduced: number;
  status: number;
  startDate: string;
  endDate?: string;
  product?: {
    description: string;
  };
}

export interface CreateProductionOrderDto {
  orderNumber: string;
  productCode: string;
  quantityPlanned: number;
  status: number;
  startDate: string;
}

export interface UpdateProductionOrderDto {
  orderNumber?: string;
  productCode?: string;
  quantityPlanned?: number;
  status?: number;
  startDate?: string;
  endDate?: string;
}

export interface Product {
  id: number;
  code: string;
  description: string;
}
