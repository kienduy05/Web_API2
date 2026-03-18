import flask
from config.db import get_connection

customer_bp = flask.Blueprint("customer_bp", __name__)


# GET ALL
@customer_bp.route('/customer/getall', methods=['GET'])
def get_all_customer():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT CustomerID, CustomerName, CustomerGender,
                   CustomerPhone, CustomerEmail, CustomerAddress
            FROM Customer
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# GET BY ID
@customer_bp.route('/customer/getbyid/<id>', methods=['GET'])
def get_customer_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT CustomerID, CustomerName, CustomerGender,
                   CustomerPhone, CustomerEmail, CustomerAddress
            FROM Customer
            WHERE CustomerID = ?
        """, (id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# ADD
@customer_bp.route('/customer/add', methods=['POST'])
def add_customer():
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        command = """
        INSERT INTO Customer
        (CustomerName, CustomerGender, CustomerPhone, CustomerEmail, CustomerAddress)
        VALUES (?, ?, ?, ?, ?)
        """

        cursor.execute(command, (
            data["CustomerName"],
            data["CustomerGender"],
            data["CustomerPhone"],
            data["CustomerEmail"],
            data["CustomerAddress"]
        ))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "them thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# UPDATE
@customer_bp.route('/customer/update', methods=['PUT'])
def update_customer():
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        command = """
        UPDATE Customer
        SET CustomerName=?,
            CustomerGender=?,
            CustomerPhone=?,
            CustomerEmail=?,
            CustomerAddress=?
        WHERE CustomerID=?
        """

        cursor.execute(command, (
            data["CustomerName"],
            data["CustomerGender"],
            data["CustomerPhone"],
            data["CustomerEmail"],
            data["CustomerAddress"],
            data["CustomerID"]
        ))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "cap nhat thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# DELETE
@customer_bp.route('/customer/delete/<id>', methods=['DELETE'])
def delete_customer(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM Customer WHERE CustomerID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "xoa thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

@customer_bp.route('/customer/check-email/<email>', methods=['GET'])
def check_email(email):
    conn = get_connection()
    cursor = conn.cursor()

    cursor.execute("SELECT CustomerEmail FROM Customer WHERE CustomerEmail = ?", (email,))
    result = cursor.fetchone()

    cursor.close()
    conn.close()

    return flask.jsonify({"exists": result is not None})

@customer_bp.route('/customer/check-phone/<phone>', methods=['GET'])
def check_phone(phone):
    conn = get_connection()
    cursor = conn.cursor()

    cursor.execute("SELECT CustomerPhone FROM Customer WHERE CustomerPhone = ?", (phone,))
    result = cursor.fetchone()

    cursor.close()
    conn.close()

    return flask.jsonify({"exists": result is not None})

@customer_bp.route('/customer/search', methods=['GET'])
def search_customer():
    try:
        keyword = flask.request.args.get("keyword", "")
        type_search = flask.request.args.get("type", "")

        conn = get_connection()
        cursor = conn.cursor()

        if type_search == "name":
            query = "SELECT * FROM Customer WHERE CustomerName LIKE ?"
        elif type_search == "email":
            query = "SELECT * FROM Customer WHERE CustomerEmail LIKE ?"
        elif type_search == "phone":
            query = "SELECT * FROM Customer WHERE CustomerPhone LIKE ?"
        else:
            return flask.jsonify([])

        cursor.execute(query, (f"%{keyword}%",))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500