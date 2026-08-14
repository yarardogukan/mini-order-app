import { useEffect, useState } from "react";
import {
  deleteCategory,
  getCategories,
  getCategoryById,
} from "../../../api/categoryApi";
import CategoryFormModal from "../../../components/admin/categories/CategoryFormModal";
import DeleteCategoryModal from "../../../components/admin/categories/DeleteCategoryModal";
import type { Category, CategoryDetail } from "../../../types/category";

function AdminCategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [categoryModalMode, setCategoryModalMode] = useState<
    "create" | "edit" | null
  >(null);

  const [selectedCategory, setSelectedCategory] =
    useState<CategoryDetail | null>(null);

  const [loadingCategoryId, setLoadingCategoryId] = useState<number | null>(
    null
  );

  const [categoryToDelete, setCategoryToDelete] = useState<{
    id: number;
    name: string;
  } | null>(null);

  const [deletingCategoryId, setDeletingCategoryId] = useState<number | null>(
    null
  );

  const loadCategories = async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await getCategories();

      setCategories(data);
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Categories could not be loaded."
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  const categoryRows = categories.flatMap((category) => [
    {
      id: category.id,
      name: category.name,
      slug: category.slug,
      parentName: null,
      type: "Root",
    },
    ...category.subCategories.map((subCategory) => ({
      id: subCategory.id,
      name: subCategory.name,
      slug: subCategory.slug,
      parentName: category.name,
      type: "Subcategory",
    })),
  ]);

  return (
    <main className="admin-categories-page">
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

      <section className="admin-page-header">
        <div>
          <span className="admin-dashboard-eyebrow">Catalog</span>

          <h1>Categories</h1>

          <p>
            Manage root categories and subcategories used across the catalog.
          </p>
        </div>

        <button
          type="button"
          className="admin-primary-action"
          onClick={() => {
            setSuccessMessage(null);
            setSelectedCategory(null);
            setCategoryModalMode("create");
          }}
        >
          + Add Category
        </button>
      </section>

      {error && (
        <div className="admin-page-error" role="alert">
          {error}
        </div>
      )}

      <section className="admin-category-table-card">
        {loading ? (
          <div className="admin-table-state">Loading categories...</div>
        ) : categoryRows.length === 0 ? (
          <div className="admin-table-state">No categories found.</div>
        ) : (
          <div className="admin-category-table">
            <div className="admin-category-table-header">
              <span>Name</span>
              <span>Slug</span>
              <span>Parent</span>
              <span>Type</span>
              <span>Actions</span>
            </div>

            <div className="admin-category-table-body">
              {categoryRows.map((category) => (
                <div key={category.id} className="admin-category-table-row">
                  <div className="admin-category-name">
                    <strong>{category.name}</strong>
                  </div>

                  <span>{category.slug}</span>

                  <span>{category.parentName ?? "—"}</span>

                  <span
                    className={
                      category.type === "Root"
                        ? "admin-category-type root"
                        : "admin-category-type subcategory"
                    }
                  >
                    {category.type}
                  </span>

                  <div className="admin-category-actions">
                    <button
                      type="button"
                      disabled={loadingCategoryId === category.id}
                      onClick={async () => {
                        try {
                          setSuccessMessage(null);
                          setLoadingCategoryId(category.id);
                          setError(null);

                          const detail = await getCategoryById(category.id);

                          setSelectedCategory(detail);
                          setCategoryModalMode("edit");
                        } catch (error) {
                          setError(
                            error instanceof Error
                              ? error.message
                              : "Category could not be loaded."
                          );
                        } finally {
                          setLoadingCategoryId(null);
                        }
                      }}
                    >
                      {loadingCategoryId === category.id
                        ? "Loading..."
                        : "Edit"}
                    </button>

                    <button
                      type="button"
                      className="danger"
                      disabled={deletingCategoryId === category.id}
                      onClick={() => {
                        setError(null);
                        setSuccessMessage(null);

                        setCategoryToDelete({
                          id: category.id,
                          name: category.name,
                        });
                      }}
                    >
                      Delete
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </section>

      {categoryToDelete && (
        <DeleteCategoryModal
          categoryName={categoryToDelete.name}
          deleting={deletingCategoryId === categoryToDelete.id}
          onClose={() => setCategoryToDelete(null)}
          onConfirm={async () => {
            try {
              setDeletingCategoryId(categoryToDelete.id);
              setError(null);
              setSuccessMessage(null);

              await deleteCategory(categoryToDelete.id);

              await loadCategories();

              setSuccessMessage(
                `"${categoryToDelete.name}" was deleted successfully.`
              );

              setCategoryToDelete(null);
            } catch (error) {
              setError(
                error instanceof Error
                  ? error.message
                  : "Category could not be deleted."
              );

              setCategoryToDelete(null);
            } finally {
              setDeletingCategoryId(null);
            }
          }}
        />
      )}

      {categoryModalMode && (
        <CategoryFormModal
          mode={categoryModalMode}
          categories={categories}
          category={selectedCategory}
          onSaved={async (message) => {
            await loadCategories();
            setSuccessMessage(message);
          }}
          onClose={() => {
            setCategoryModalMode(null);
            setSelectedCategory(null);
          }}
        />
      )}
    </main>
  );
}

export default AdminCategoriesPage;
