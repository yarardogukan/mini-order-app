import { useEffect, useState } from "react";
import { createCategory, updateCategory } from "../../../api/categoryApi";
import type { Category, CategoryDetail } from "../../../types/category";

interface CategoryFormModalProps {
  mode: "create" | "edit";
  categories: Category[];
  category?: CategoryDetail | null;
  onClose: () => void;
  onSaved: (message: string) => Promise<void>;
}

function CategoryFormModal({
  mode,
  categories,
  category,
  onClose,
  onSaved,
}: CategoryFormModalProps) {
  const isEditMode = mode === "edit";

  const [name, setName] = useState(category?.name ?? "");
  const [slug, setSlug] = useState(category?.slug ?? "");
  const [parentCategoryId, setParentCategoryId] = useState<number | null>(
    category?.parentCategoryId ?? null
  );
  const [isActive, setIsActive] = useState(category?.isActive ?? true);

  const [formError, setFormError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const previousOverflow = document.body.style.overflow;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && !submitting) {
        onClose();
      }
    };

    document.body.style.overflow = "hidden";

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      document.body.style.overflow = previousOverflow;

      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [onClose, submitting]);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (submitting) {
      return;
    }

    const trimmedName = name.trim();
    const trimmedSlug = slug.trim();

    if (!trimmedName) {
      setFormError("Category name is required.");
      return;
    }

    if (!trimmedSlug) {
      setFormError("Category slug is required.");
      return;
    }

    if (trimmedSlug.includes(" ")) {
      setFormError("Category slug cannot contain spaces.");
      return;
    }

    try {
      setSubmitting(true);
      setFormError(null);

      if (!isEditMode) {
        await createCategory({
          name: trimmedName,
          slug: trimmedSlug,
          parentCategoryId,
        });

        await onSaved("Category created successfully.");
        onClose();

        return;
      }

      if (!category) {
        setFormError("Category could not be loaded.");
        return;
      }

      await updateCategory(category.id, {
        name: trimmedName,
        slug: trimmedSlug,
        parentCategoryId,
        isActive,
      });

      await onSaved("Category updated successfully.");
      onClose();
    } catch (error) {
      setFormError(
        error instanceof Error ? error.message : "Category could not be saved."
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleClose = () => {
    if (submitting) {
      return;
    }

    onClose();
  };

  return (
    <div
      className="admin-modal-backdrop"
      role="presentation"
      onClick={handleClose}
    >
      <section
        className="admin-form-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="category-form-title"
        onClick={(event) => event.stopPropagation()}
      >
        <header className="admin-form-modal-header">
          <div>
            <span className="admin-dashboard-eyebrow">Catalog</span>

            <h2 id="category-form-title">
              {isEditMode ? "Edit Category" : "Add Category"}
            </h2>

            <p>
              {isEditMode
                ? "Update category information and hierarchy."
                : "Create a new root category or subcategory."}
            </p>
          </div>

          <button
            type="button"
            className="admin-modal-close"
            aria-label="Close"
            disabled={submitting}
            onClick={handleClose}
          >
            ×
          </button>
        </header>

        <form className="admin-category-form" onSubmit={handleSubmit}>
          <label>
            <span>Name</span>

            <input
              type="text"
              placeholder="Category name"
              value={name}
              disabled={submitting}
              onChange={(event) => {
                setName(event.target.value);
                setFormError(null);
              }}
            />
          </label>

          <label>
            <span>Slug</span>

            <input
              type="text"
              placeholder="category-slug"
              value={slug}
              disabled={submitting}
              onChange={(event) => {
                setSlug(event.target.value);
                setFormError(null);
              }}
            />
          </label>

          <label>
            <span>Parent Category</span>

            <select
              value={parentCategoryId?.toString() ?? ""}
              disabled={submitting}
              onChange={(event) => {
                setParentCategoryId(
                  event.target.value ? Number(event.target.value) : null
                );

                setFormError(null);
              }}
            >
              <option value="">No parent — Root category</option>

              {categories.map((parentCategory) => (
                <option key={parentCategory.id} value={parentCategory.id}>
                  {parentCategory.name}
                </option>
              ))}
            </select>
          </label>

          {isEditMode && (
            <label className="admin-category-active-field">
              <span>Status</span>

              <select
                value={isActive ? "active" : "inactive"}
                disabled={submitting}
                onChange={(event) => {
                  setIsActive(event.target.value === "active");

                  setFormError(null);
                }}
              >
                <option value="active">Active</option>

                <option value="inactive">Inactive</option>
              </select>
            </label>
          )}

          {formError && (
            <div className="admin-form-error" role="alert">
              {formError}
            </div>
          )}

          <div className="admin-form-modal-actions">
            <button
              type="button"
              className="admin-secondary-action"
              disabled={submitting}
              onClick={handleClose}
            >
              Cancel
            </button>

            <button
              type="submit"
              className="admin-primary-action"
              disabled={submitting}
            >
              {submitting
                ? isEditMode
                  ? "Saving..."
                  : "Creating..."
                : isEditMode
                ? "Save Changes"
                : "Create Category"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}

export default CategoryFormModal;
