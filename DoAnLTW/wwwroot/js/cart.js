document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-plus, .btn-minus").forEach(button => {
        button.addEventListener("click", function () {
            let row = this.closest("tr");
            let productId = row.dataset.productId;
            let size = row.dataset.size;
            let color = row.dataset.color;
            let action = this.classList.contains("btn-plus") ? "increase" : "decrease";

            fetch("/Cart/UpdateCart", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ productId, size, color, action })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        row.querySelector(".quantity-input").value = data.itemTotal / row.dataset.price;
                        row.querySelector(".item-total").textContent = data.itemTotal.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
                        document.getElementById("cart-count").textContent = data.cartCount;
                        document.getElementById("total-price").textContent = data.totalPrice.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
                    }
                });
        });
    });

    document.querySelectorAll(".btn-delete").forEach(button => {
        button.addEventListener("click", function () {
            let row = this.closest("tr");
            let productId = row.dataset.productId;
            let size = row.dataset.size;
            let color = row.dataset.color;

            fetch("/Cart/RemoveFromCart", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ productId, size, color })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        row.remove();
                        document.getElementById("cart-count").textContent = data.cartCount;
                        document.getElementById("total-price").textContent = data.totalPrice.toLocaleString("vi-VN", { style: "currency", currency: "VND" });
                    }
                });
        });
    });
});
