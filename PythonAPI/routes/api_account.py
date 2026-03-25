import flask
from config.db import get_connection

account_bp = flask.Blueprint("account_bp", __name__)


# LOGIN
@account_bp.route('/account/login', methods=['POST'])
def login():
    try:
        data = flask.request.json

        username = data.get("Username")
        password = data.get("Password")

        conn = get_connection()
        cursor = conn.cursor()

        command = """
        SELECT AccountID, Username, AccountType,
               AccountDisplayName, CustomerID
        FROM Account
        WHERE Username=? AND Password=?
        """

        cursor.execute(command, (username, password))

        keys = [col[0] for col in cursor.description]
        result = cursor.fetchone()

        cursor.close()
        conn.close()

        if result:
            return flask.jsonify(dict(zip(keys, result)))

        return flask.jsonify({"mess": "sai tai khoan hoac mat khau"}), 401

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# GET ALL
@account_bp.route('/account/getall', methods=['GET'])
def get_all_account():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
        SELECT AccountID, Username, AccountType,
               AccountDisplayName, CustomerID
        FROM Account
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET ALL WITH CUSTOMER
@account_bp.route('/account/getall-with-customer', methods=['GET'])
def get_all_account_with_customer():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
        SELECT 
            a.AccountID, 
            a.Username, 
            a.AccountType,
            a.AccountDisplayName, 
            a.CustomerID,
            c.CustomerName
        FROM Account a
        LEFT JOIN Customer c 
            ON a.CustomerID = c.CustomerID
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET CUSTOMER CHUA CO ACCOUNT
@account_bp.route('/account/get-customer-no-account', methods=['GET'])
def get_customer_no_account():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
        SELECT CustomerID, CustomerName
        FROM Customer
        WHERE CustomerID NOT IN (
            SELECT CustomerID 
            FROM Account 
            WHERE CustomerID IS NOT NULL
        )
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# ADD
@account_bp.route('/account/add', methods=['POST'])
def add_account():
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        command = """
        INSERT INTO Account
        (Username, Password, AccountType, AccountDisplayName, CustomerID)
        VALUES (?, ?, ?, ?, ?)
        """

        cursor.execute(command, (
            data["Username"],
            data["Password"],
            data["AccountType"],
            data["AccountDisplayName"],
            data["CustomerID"]
        ))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "them thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# UPDATE
@account_bp.route('/account/update', methods=['PUT'])
def update_account():
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        command = """
        UPDATE Account
        SET Password=?,
            AccountType=?,
            AccountDisplayName=?,
            CustomerID=?
        WHERE AccountID=?
        """

        cursor.execute(command, (
            data["Password"],
            data["AccountType"],
            data["AccountDisplayName"],
            data["CustomerID"],
            data["AccountID"]
        ))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "cap nhat thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# DELETE
@account_bp.route('/account/delete/<id>', methods=['DELETE'])
def delete_account(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # check account tồn tại
        cursor.execute("SELECT AccountID FROM Account WHERE AccountID=?", (id,))
        if not cursor.fetchone():
            cursor.close()
            conn.close()
            return flask.jsonify({"mess": "Tài khoản không tồn tại"}), 404

        # ✅ xóa bình thường
        cursor.execute("DELETE FROM Account WHERE AccountID=?", (id,))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "Xóa tài khoản thành công"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

@account_bp.route('/account/check-username/<username>', methods=['GET'])
def check_username(username):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT Username FROM Account WHERE Username = ?", (username,))
        result = cursor.fetchone()

        cursor.close()
        conn.close()

        if result:
            return flask.jsonify({"exists": True})
        return flask.jsonify({"exists": False})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# RESET PASSWORD
@account_bp.route('/account/reset-password/<id>', methods=['PUT'])
def reset_password(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        # kiểm tra account tồn tại
        cursor.execute("SELECT AccountID FROM Account WHERE AccountID=?", (id,))
        if not cursor.fetchone():
            return flask.jsonify({"mess": "Tài khoản không tồn tại"}), 404

        # reset password = 123456
        cursor.execute("""
            UPDATE Account
            SET Password=?
            WHERE AccountID=?
        """, ("123456", id))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "Đã reset mật khẩu về 123456"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

@account_bp.route('/account/register', methods=['POST'])
def register():
    try:
        data = flask.request.json

        conn = get_connection()
        cursor = conn.cursor()

        # CHECK USERNAME
        cursor.execute("SELECT Username FROM Account WHERE Username=?", (data["Username"],))
        if cursor.fetchone():
            return flask.jsonify({"success": False, "mess": "Username đã tồn tại"}), 400

        # CHECK PHONE
        cursor.execute("SELECT CustomerPhone FROM Customer WHERE CustomerPhone=?", (data["CustomerPhone"],))
        if cursor.fetchone():
            return flask.jsonify({"success": False, "mess": "Số điện thoại đã tồn tại"}), 400

        # CHECK EMAIL
        cursor.execute("SELECT CustomerEmail FROM Customer WHERE CustomerEmail=?", (data["CustomerEmail"],))
        if cursor.fetchone():
            return flask.jsonify({"success": False, "mess": "Email đã tồn tại"}), 400

        # INSERT CUSTOMER
        cursor.execute("""
            INSERT INTO Customer (CustomerName, CustomerPhone, CustomerEmail, CustomerAddress)
            OUTPUT INSERTED.CustomerID
            VALUES (?, ?, ?, ?)
        """, (
            data["CustomerName"],
            data["CustomerPhone"],
            data["CustomerEmail"],
            data["CustomerAddress"]
        ))

        customer_id = cursor.fetchone()[0]

        # INSERT ACCOUNT
        cursor.execute("""
            INSERT INTO Account (Username, Password, AccountType, AccountDisplayName, CustomerID)
            VALUES (?, ?, ?, ?, ?)
        """, (
            data["Username"],
            data["Password"],
            2,
            data["CustomerName"],
            customer_id
        ))

        conn.commit()
        cursor.close()
        conn.close()

        return flask.jsonify({"success": True, "mess": "Đăng ký thành công"})

    except Exception as e:
        return flask.jsonify({"success": False, "mess": str(e)}), 500

@account_bp.route('/account/change-password/<id>', methods=['PUT'])
def change_password(id):
    try:
        data = flask.request.json

        old_password = data.get("OldPassword")
        new_password = data.get("NewPassword")

        # ✅ validate null
        if not old_password or not new_password:
            return flask.jsonify({
                "success": False,
                "mess": "Vui lòng nhập đầy đủ thông tin"
            }), 400

        # ✅ validate >= 6 ký tự
        if len(new_password) < 6:
            return flask.jsonify({
                "success": False,
                "mess": "Mật khẩu phải có ít nhất 6 ký tự"
            }), 400

        conn = get_connection()
        cursor = conn.cursor()

        # check account tồn tại
        cursor.execute("SELECT Password FROM Account WHERE AccountID=?", (id,))
        row = cursor.fetchone()

        if not row:
            return flask.jsonify({
                "success": False,
                "mess": "Tài khoản không tồn tại"
            }), 404

        # check password cũ
        if row[0] != old_password:
            return flask.jsonify({
                "success": False,
                "mess": "Mật khẩu cũ không đúng"
            }), 400

        # update password
        cursor.execute("""
            UPDATE Account
            SET Password=?
            WHERE AccountID=?
        """, (new_password, id))

        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({
            "success": True,
            "mess": "Đổi mật khẩu thành công"
        })

    except Exception as e:
        return flask.jsonify({
            "success": False,
            "mess": str(e)
        }), 500