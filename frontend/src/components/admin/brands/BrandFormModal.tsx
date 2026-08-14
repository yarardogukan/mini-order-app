import { useEffect, useState } from "react";
import { createBrand, updateBrand } from "../../../api/brandApi";
import type { BrandDetail } from "../../../types/brand";

interface BrandFormModalProps {
  mode: "create" | "edit";
  brand?: BrandDetail | null;
  onClose: () => void;
  onSaved: (message: string) => Promise<void>;
}

function BrandFormModal({
  mode,
  brand,
  onClose,
  onSaved,
}: BrandFormModalProps) {
  const isEditMode = mode === "edit";

  const [name, setName] = useState(brand?.name ?? "");
  const [slug, setSlug] = useState(brand?.slug ?? "");
  const [isActive, setIsActive] = useState(brand?.isActive ?? true);

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

  const handleClose = () => {
    if (submitting) {
      return;
    }

    onClose();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (submitting) {
      return;
    }

    const trimmedName = name.trim();
    const trimmedSlug = slug.trim();

    if (!trimmedName) {
      setFormError("Brand name is required.");
      return;
    }

    if (!trimmedSlug) {
      setFormError("Brand slug is required.");
      return;
    }

    if (trimmedSlug.includes(" ")) {
      setFormError("Brand slug cannot contain spaces.");
      return;
    }

    try {
      setSubmitting(true);
      setFormError(null);

      if (!isEditMode) {
        await createBrand({
          name: trimmedName,
          slug: trimmedSlug,
        });

        await onSaved("Brand created successfully.");

        onClose();

        return;
      }

      if (!brand) {
        setFormError("Brand could not be loaded.");
        return;
      }

      await updateBrand(brand.id, {
        name: trimmedName,
        slug: trimmedSlug,
        isActive,
      });

      await onSaved("Brand updated successfully.");

      onClose();
    } catch (error) {
      setFormError(
        error instanceof Error ? error.message : "Brand could not be saved."
      );
    } finally {
      setSubmitting(false);
    }
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
        aria-labelledby="brand-form-title"
        onClick={(event) => event.stopPropagation()}
      >
        <header className="admin-form-modal-header">
          <div>
            <span className="admin-dashboard-eyebrow">Catalog</span>

            <h2 id="brand-form-title">
              {isEditMode ? "Edit Brand" : "Add Brand"}
            </h2>

            <p>
              {isEditMode
                ? "Update brand information and availability."
                : "Create a new brand for the product catalog."}
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
              placeholder="Brand name"
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
              placeholder="brand-slug"
              value={slug}
              disabled={submitting}
              onChange={(event) => {
                setSlug(event.target.value);
                setFormError(null);
              }}
            />
          </label>

          {isEditMode && (
            <label>
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
                : "Create Brand"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}

export default BrandFormModal;
