import flask
from config.db import get_connection

author_bp = flask.Blueprint("author_bp", __name__)

# 1. GET ALL
@author_bp.route('/author/getall', methods=['GET'])
def get_all_author():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM Author")

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()
        return flask.jsonify(results)
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 2. GET BY ID
@author_bp.route('/author/getbyid/<id>', methods=['GET'])
def get_author_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("SELECT * FROM Author WHERE AuthorID = ?", (id,))

        keys = [col[0] for col in cursor.description]
        row = cursor.fetchone()

        cursor.close()
        conn.close()

        if row:
            return flask.jsonify(dict(zip(keys, row)))
        return flask.jsonify({"mess": "Khong tim thay tac gia"}), 404
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 3. INSERT
@author_bp.route('/author/add', methods=['POST'])
def add_author():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = "INSERT INTO Author (AuthorName) VALUES (?)"
        params = (data.get("AuthorName"),)

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Them tac gia thanh cong"}), 201
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 4. UPDATE
@author_bp.route('/author/update', methods=['PUT'])
def update_author():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
                  UPDATE Author
                  SET AuthorName = ?
                  WHERE AuthorID = ?
                  """
        params = (data["AuthorName"], data["AuthorID"])

        cursor.execute(command, params)
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Cap nhat tac gia thanh cong"})
    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500


# 5. DELETE
@author_bp.route('/author/delete/<id>', methods=['DELETE'])
def delete_author(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM Author WHERE AuthorID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()
        return flask.jsonify({"mess": "Xoa tac gia thanh cong"})
    except Exception as e:
        return flask.jsonify({"mess": "Khong the xoa vi tac gia dang duoc su dung", "error": str(e)}), 400