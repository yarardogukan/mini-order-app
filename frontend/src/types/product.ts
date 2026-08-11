export interface Product {
  id: number;
  stockCode: string;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  categoryName: string;
  brandName: string;
  coverImageUrl: string | null;
}

export interface ProductImage {
  imageUrl: string;
  isCover: boolean;
  sortOrder: number;
}

export interface ProductAttribute {
  name: string;
  code: string;
  dataType: string;
  value: string;
  sortOrder: number;
}

export interface ProductDetail {
  id: number;
  stockCode: string;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId: number;
  categoryName: string;
  parentCategoryName: string | null;
  brandId: number;
  brandName: string;
  images: ProductImage[];
  attributes: ProductAttribute[];
}
