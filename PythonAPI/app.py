import flask
from flask_cors import CORS

from routes.api_bookcategory import category_bp
from routes.api_customer import customer_bp
from routes.api_account import account_bp
from routes.api_publisher import publisher_bp
from routes.api_orders import orders_bp
from routes.api_orderdetail import orderdetail_bp
from routes.api_book import book_bp
from routes.api_author import author_bp
app = flask.Flask(__name__)
CORS(app)

app.register_blueprint(customer_bp)
app.register_blueprint(account_bp)
app.register_blueprint(category_bp)
app.register_blueprint(publisher_bp)
app.register_blueprint(orders_bp)
app.register_blueprint(orderdetail_bp)
app.register_blueprint(author_bp)
app.register_blueprint(book_bp)
@app.route('/')
def home():
    return {"mess": "Flask API BTL_API dang chay"}


if __name__ == "__main__":
    app.run()