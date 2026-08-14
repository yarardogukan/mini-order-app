import { useEffect, useState } from "react";
import { deleteBrand, getBrandById, getBrands } from "../../../api/brandApi";
import BrandFormModal from "../../../components/admin/brands/BrandFormModal";
import DeleteBrandModal from "../../../components/admin/brands/DeleteBrandModal";
import type { Brand, BrandDetail } from "../../../types/brand";

function AdminBrandsPage() {
  const [brands, setBrands] = useState<Brand[]>([]);
  const [loading, setLoading] = useState(true);

  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [brandModalMode, setBrandModalMode] = useState<
    "create" | "edit" | null
  >(null);

  const [selectedBrand, setSelectedBrand] = useState<BrandDetail | null>(null);

  const [loadingBrandId, setLoadingBrandId] = useState<number | null>(null);

  const [brandToDelete, setBrandToDelete] = useState<{
    id: number;
    name: string;
  } | null>(null);

  const [deletingBrandId, setDeletingBrandId] = useState<number | null>(null);

  const loadBrands = async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await getBrands();

      setBrands(data);
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Brands could not be loaded."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBrands();
  }, []);

  const handleAddBrand = () => {
    setError(null);
    setSuccessMessage(null);

    setSelectedBrand(null);
    setBrandModalMode("create");
  };

  const handleEditBrand = async (id: number) => {
    try {
      setError(null);
      setSuccessMessage(null);

      setLoadingBrandId(id);

      const detail = await getBrandById(id);

      setSelectedBrand(detail);
      setBrandModalMode("edit");
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Brand could not be loaded."
      );
    } finally {
      setLoadingBrandId(null);
    }
  };

  const handleDeleteRequest = (id: number, name: string) => {
    setError(null);
    setSuccessMessage(null);

    setBrandToDelete({
      id,
      name,
    });
  };

  const handleDeleteConfirm = async () => {
    if (!brandToDelete) {
      return;
    }

    try {
      setDeletingBrandId(brandToDelete.id);

      setError(null);
      setSuccessMessage(null);

      await deleteBrand(brandToDelete.id);

      await loadBrands();

      setSuccessMessage(`"${brandToDelete.name}" was deleted successfully.`);

      setBrandToDelete(null);
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Brand could not be deleted."
      );

      setBrandToDelete(null);
    } finally {
      setDeletingBrandId(null);
    }
  };

  return (
    <main className="admin-brands-page">
      <section className="admin-page-header">
        <div>
          <span className="admin-dashboard-eyebrow">Catalog</span>

          <h1>Brands</h1>

          <p>Manage brands used across the MiniOrder product catalog.</p>
        </div>

        <button
          type="button"
          className="admin-primary-action"
          onClick={handleAddBrand}
        >
          + Add Brand
        </button>
      </section>

      {successMessage && (
        <div className="admin-page-success" role="status">
          <span>{successMessage}</span>

          <button
            type="button"
            aria-label="Dismiss success message"
            onClick={() => setSuccessMessage(null)}
          >
            ×
          </button>
        </div>
      )}

      {error && (
        <div className="admin-page-error" role="alert">
          {error}
        </div>
      )}

      <section className="admin-brand-table-card">
        {loading ? (
          <div className="admin-table-state">Loading brands...</div>
        ) : brands.length === 0 ? (
          <div className="admin-table-state">No brands found.</div>
        ) : (
          <div className="admin-brand-table">
            <div className="admin-brand-table-header">
              <span>Name</span>
              <span>Slug</span>
              <span>Actions</span>
            </div>

            <div className="admin-brand-table-body">
              {brands.map((brand) => (
                <div key={brand.id} className="admin-brand-table-row">
                  <div className="admin-brand-name">
                    <strong>{brand.name}</strong>
                  </div>

                  <span>{brand.slug}</span>

                  <div className="admin-brand-actions">
                    <button
                      type="button"
                      disabled={loadingBrandId === brand.id}
                      onClick={() => handleEditBrand(brand.id)}
                    >
                      {loadingBrandId === brand.id ? "Loading..." : "Edit"}
                    </button>

                    <button
                      type="button"
                      className="danger"
                      disabled={deletingBrandId === brand.id}
                      onClick={() => handleDeleteRequest(brand.id, brand.name)}
                    >
                      {deletingBrandId === brand.id ? "Deleting..." : "Delete"}
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </section>

      {brandModalMode && (
        <BrandFormModal
          mode={brandModalMode}
          brand={selectedBrand}
          onSaved={async (message) => {
            await loadBrands();

            setSuccessMessage(message);
          }}
          onClose={() => {
            setBrandModalMode(null);
            setSelectedBrand(null);
          }}
        />
      )}

      {brandToDelete && (
        <DeleteBrandModal
          brandName={brandToDelete.name}
          deleting={deletingBrandId === brandToDelete.id}
          onClose={() => setBrandToDelete(null)}
          onConfirm={handleDeleteConfirm}
        />
      )}
    </main>
  );
}

export default AdminBrandsPage;
