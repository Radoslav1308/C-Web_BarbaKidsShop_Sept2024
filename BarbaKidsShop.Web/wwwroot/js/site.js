document.addEventListener("DOMContentLoaded", () => {
    // Handle quantity decrease
    document.querySelectorAll('.quantity-decrease').forEach(button => {
        button.addEventListener('click', () => {
            const productId = button.getAttribute('data-product-id');
            const input = document.querySelector(`.quantity-input[data-product-id='${productId}']`);
            const newValue = Math.max(1, parseInt(input.value) - 1);
            input.value = newValue;
            updateQuantity(productId, newValue);
        });
    });

    // Handle quantity increase
    document.querySelectorAll('.quantity-increase').forEach(button => {
        button.addEventListener('click', () => {
            const productId = button.getAttribute('data-product-id');
            const input = document.querySelector(`.quantity-input[data-product-id='${productId}']`);
            const newValue = parseInt(input.value) + 1;
            input.value = newValue;
            updateQuantity(productId, newValue);
        });
    });

    // Handle manual input change
    document.querySelectorAll('.quantity-input').forEach(input => {
        input.addEventListener('change', () => {
            const productId = input.getAttribute('data-product-id');
            const newValue = Math.max(1, parseInt(input.value) || 1);
            input.value = newValue;
            updateQuantity(productId, newValue);
        });
    });

    // Update quantity function (AJAX)
    const updateQuantity = (productId, quantity) => {
        fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ productId, quantity })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to update quantity');
                }
                return response.json();
            })
            .then(data => {
                // Optionally, update the total price or other UI elements
                console.log('Quantity updated:', data);
                // Reload page or update UI dynamically here
            })
            .catch(error => {
                console.error(error);
                alert('Failed to update quantity. Please try again.');
            });
    };
});
