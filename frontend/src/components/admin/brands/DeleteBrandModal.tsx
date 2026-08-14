import { useEffect, useState } from "react";

interface DeleteBrandModalProps {
  brandName: string;
  deleting: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
}

function DeleteBrandModal({
  brandName,
  deleting,
  onClose,
  onConfirm,
}: DeleteBrandModalProps) {
  const [confirmationText, setConfirmationText] = useState("");

  const isConfirmed = confirmationText === brandName;

  useEffect(() => {
    const previousOverflow = document.body.style.overflow;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && !deleting) {
        onClose();
      }
    };

    document.body.style.overflow = "hidden";

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      document.body.style.overflow = previousOverflow;

      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [deleting, onClose]);

  const handleClose = () => {
    if (!deleting) {
      onClose();
    }
  };

  return (
    <div
      className="admin-modal-backdrop"
      role="presentation"
      onClick={handleClose}
    >
      <section
        className="admin-delete-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="delete-brand-title"
        onClick={(event) => event.stopPropagation()}
      >
        <header className="admin-delete-modal-header">
          <div>
            <span className="admin-delete-eyebrow">Danger zone</span>

            <h2 id="delete-brand-title">Delete brand</h2>
          </div>

          <button
            type="button"
            className="admin-modal-close"
            aria-label="Close"
            disabled={deleting}
            onClick={handleClose}
          >
            ×
          </button>
        </header>

        <div className="admin-delete-modal-content">
          <div className="admin-delete-warning">
            <div className="admin-delete-warning-icon" aria-hidden="true">
              !
            </div>

            <div className="admin-delete-warning-content">
              <strong>Are you sure you want to delete this brand?</strong>

              <p>
                This action is permanent and cannot be undone. The brand will
                only be removed if no active dependencies prevent deletion.
              </p>
            </div>
          </div>

          <div className="admin-delete-category-name">
            <span>Brand to delete</span>

            <strong>{brandName}</strong>
          </div>

          <label className="admin-delete-confirmation-field">
            <span>
              To confirm, type <strong>{brandName}</strong> below.
            </span>

            <input
              type="text"
              value={confirmationText}
              disabled={deleting}
              autoComplete="off"
              onChange={(event) => setConfirmationText(event.target.value)}
            />
          </label>
        </div>

        <footer className="admin-delete-modal-actions">
          <button
            type="button"
            className="admin-secondary-action"
            disabled={deleting}
            onClick={handleClose}
          >
            Cancel
          </button>

          <button
            type="button"
            className="admin-danger-action"
            disabled={!isConfirmed || deleting}
            onClick={onConfirm}
          >
            {deleting ? "Deleting..." : "Delete this brand"}
          </button>
        </footer>
      </section>
    </div>
  );
}

export default DeleteBrandModal;
