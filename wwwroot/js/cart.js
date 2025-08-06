const addCartBtn = document.getElementById("add-cart-btn");
const addCart = document.getElementById("add-cart");
const itemCount = document.getElementById("item-count");

let item_count = 0;

function addToCart(productId, quantity, isLogin=false) {
    if (!isLogin) {
        window.location = "/account/login";
    }
    else {
        item_count = quantity;
        addCart.style.display = 'flex';
        addCartBtn.style.display = 'none';
        item_count++;
        itemCount.innerText = item_count;
        sendToServerCart(productId);
    }
}

function addToCartOne(productId, available) {
    if (item_count < available) {
        item_count++;
        itemCount.innerText = item_count;
        sendToServerCart(productId);
    }
}

function removeToCart(productId) {
    item_count--;

    if (item_count > 0) {
        itemCount.innerText = item_count;
    }
    else {
        addCart.style.display = 'none';
        addCartBtn.style.display = 'block';
    }
    decreaseQuantity(productId)
}


//udate functions
function addToCartOneReload(productId, available, quantity) {
    if (quantity < available) {
        sendToServerCart(productId);
        setTimeout(function () {
            location.reload();
        }, 500);
    }
}

function removeToCartReload(productId, quantity) {
    if (quantity > 0) {
        decreaseQuantity(productId)
        setTimeout(function () {
            location.reload();
        }, 1000);
    }
}

function removeFromCartReload(productId) {
    removeFromCart(productId);
    setTimeout(function () {
        location.reload();
    }, 500);
}


// add to cart
function sendToServerCart(productId) {
    fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            productId: productId,
            quantity: 1
        })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                console.log(data.message);
            } else {
                alert("Xatolik yuz berdi.");
            }
        });
}

//kamaytirish
function decreaseQuantity(productId) {
    fetch('/Cart/DecreaseQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ productId })
    })
        .then(res => res.json());
}

//o'chirish
function removeFromCart(productId) {
    fetch('/Cart/RemoveFromCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ productId })
    })
        .then(res => res.json());
}
