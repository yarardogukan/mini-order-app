export interface SubCategory {
  id: number;
  name: string;
  slug: string;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  subCategories: SubCategory[];
}

export interface CategoryDetail {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
  parentCategoryId: number | null;
  parentCategoryName: string | null;
  subCategories: SubCategory[];
}

export interface CreateCategoryRequest {
  name: string;
  slug: string;
  parentCategoryId: number | null;
}

export interface UpdateCategoryRequest {
  name: string;
  slug: string;
  parentCategoryId: number | null;
  isActive: boolean;
}
