
document.addEventListener("DOMContentLoaded", function () {

    const drawer = document.getElementById("cartDrawer");
    const overlay = document.getElementById("cartOverlay");

    const openButton = document.getElementById("openCartDrawer");
    const closeButton = document.getElementById("closeCartDrawer");

    const cartItems = document.getElementById("cartDrawerItems");
    const cartTotal = document.getElementById("cartDrawerTotal");
    const cartTitle = document.getElementById("cartDrawerTitle");

    if (!drawer || !overlay || !closeButton) {
        return;
    }

    let previousFocus = null;

    const currencyFormatter = new Intl.NumberFormat("sv-SE", {
        style: "currency",
        currency: "SEK",
        minimumFractionDigits: 2
    });

    function formatPrice(value) {
        return currencyFormatter.format(value);
    }

    function parsePrice(text) {
        if (!text) {
            return 0;
        }

        let cleaned = text
            .replace(/\s/g, "")
            .replace(/[^\d,.-]/g, "");

        if (cleaned.includes(",") && cleaned.includes(".")) {
            if (cleaned.lastIndexOf(",") > cleaned.lastIndexOf(".")) {
                cleaned = cleaned.replace(/\./g, "").replace(",", ".");
            } else {
                cleaned = cleaned.replace(/,/g, "");
            }
        } else if (cleaned.includes(",")) {
            cleaned = cleaned.replace(",", ".");
        }

        const result = Number.parseFloat(cleaned);

        return Number.isFinite(result) ? result : 0;
    }

    function showEmptyCart() {
        cartItems.innerHTML = "";

        const message = document.createElement("p");
        message.textContent = "Your shopping cart is empty.";

        cartItems.appendChild(message);

        cartTotal.textContent = formatPrice(0);
        cartTitle.textContent = "My Cart (0 products)";
    }

    function showCartError() {
        cartItems.innerHTML = "";

        const message = document.createElement("p");
        message.textContent =
            "Unable to load your cart. Please open the cart page.";

        const link = document.createElement("a");
        link.href = drawer.dataset.cartUrl || "/Cart";
        link.textContent = "Go to Cart";

        cartItems.appendChild(message);
        cartItems.appendChild(link);
    }

    function createCartItem(product) {

        const item = document.createElement("div");
        item.className = "cart-item";

        if (product.image) {
            const image = document.createElement("img");
            image.src = product.image;
            image.alt = product.name;
            image.loading = "lazy";

            item.appendChild(image);
        }

        const info = document.createElement("div");
        info.className = "cart-item-info";

        const name = document.createElement("h5");
        name.textContent = product.name;

        info.appendChild(name);

        if (product.color) {
            const color = document.createElement("p");
            color.textContent = "Color: " + product.color;

            info.appendChild(color);
        }

        const quantity = document.createElement("p");
        quantity.textContent = "Quantity: " + product.quantity;

        info.appendChild(quantity);

        const price = document.createElement("p");
        price.className = "cart-item-price";
        price.textContent = formatPrice(product.price);

        info.appendChild(price);

        item.appendChild(info);

        return item;
    }

    async function loadCartDrawer() {

        cartItems.innerHTML = "";

        const loading = document.createElement("p");
        loading.textContent = "Loading your cart...";

        cartItems.appendChild(loading);

        try {
            const cartUrl = drawer.dataset.cartUrl || "/Cart";

            const response = await fetch(cartUrl, {
                method: "GET",
                credentials: "same-origin",
                cache: "no-store"
            });

            if (!response.ok) {
                throw new Error("Cart request failed.");
            }

            const html = await response.text();

            const documentParser = new DOMParser();
            const cartDocument = documentParser.parseFromString(
                html,
                "text/html"
            );

            const rows = cartDocument.querySelectorAll(
                "table tbody tr"
            );

            if (rows.length === 0) {
                showEmptyCart();
                return;
            }

            const products = [];

            rows.forEach(function (row) {

                const cells = row.querySelectorAll("td");

                if (cells.length < 4) {
                    return;
                }

                const name = cells[0].textContent.trim();

                const price = parsePrice(cells[1].textContent);

                const quantityInput = cells[2].querySelector(
                    'input[name="quantity"]'
                );

                const quantity = quantityInput
                    ? Number.parseInt(quantityInput.value, 10) || 1
                    : Number.parseInt(cells[2].textContent, 10) || 1;

                const imageElement = cells[0].querySelector("img");

                const image = imageElement
                    ? imageElement.getAttribute("src")
                    : null;

                const colorElement = row.querySelector(
                    "[data-product-color]"
                );

                const color = colorElement
                    ? colorElement.dataset.productColor
                    : "";

                products.push({
                    name: name,
                    price: price,
                    quantity: quantity,
                    image: image,
                    color: color
                });

            });

            if (products.length === 0) {
                showEmptyCart();
                return;
            }

            cartItems.innerHTML = "";

            let total = 0;
            let productCount = 0;

            products.forEach(function (product) {

                const item = createCartItem(product);

                cartItems.appendChild(item);

                total += product.price * product.quantity;
                productCount += product.quantity;

            });

            cartTotal.textContent = formatPrice(total);

            cartTitle.textContent =
                "My Cart (" + productCount + " products)";

        } catch (error) {

            console.error("Cart drawer error:", error);

            showCartError();
        }
    }

    function openCartDrawer() {

        previousFocus = document.activeElement;

        overlay.hidden = false;

        drawer.classList.add("active");
        overlay.classList.add("active");

        drawer.setAttribute("aria-hidden", "false");

        document.body.classList.add("cart-drawer-open");

        closeButton.focus();

        loadCartDrawer();
    }

    function closeCartDrawer() {

        drawer.classList.remove("active");
        overlay.classList.remove("active");

        drawer.setAttribute("aria-hidden", "true");

        document.body.classList.remove("cart-drawer-open");

        overlay.hidden = true;

        if (previousFocus && previousFocus.focus) {
            previousFocus.focus();
        }
    }

    if (openButton) {
        openButton.addEventListener("click", function () {
            openCartDrawer();
        });
    }

    closeButton.addEventListener("click", function () {
        closeCartDrawer();
    });

    overlay.addEventListener("click", function () {
        closeCartDrawer();
    });

    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape" &&
            drawer.classList.contains("active")) {

            closeCartDrawer();
        }

    });

    window.openCartDrawer = openCartDrawer;
    window.closeCartDrawer = closeCartDrawer;
    window.refreshCartDrawer = loadCartDrawer;

});