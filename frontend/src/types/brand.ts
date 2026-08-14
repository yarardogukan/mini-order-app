export interface Brand {
  id: number;
  name: string;
  slug: string;
}

export interface BrandDetail {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
}

export interface CreateBrandRequest {
  name: string;
  slug: string;
}

export interface UpdateBrandRequest {
  name: string;
  slug: string;
  isActive: boolean;
}
