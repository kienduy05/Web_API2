import flask
from config.db import get_connection

publisher_bp = flask.Blueprint("publisher_bp", __name__)

# GET ALL
@publisher_bp.route('/publisher/getall', methods=['GET'])
def get_all_publisher():
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT PublisherID, PublisherName, PublisherAddress, PublisherPhone
            FROM Publisher
        """)

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# GET BY ID
@publisher_bp.route('/publisher/getbyid/<id>', methods=['GET'])
def get_publisher_by_id(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("""
            SELECT PublisherID, PublisherName, PublisherAddress, PublisherPhone
            FROM Publisher
            WHERE PublisherID = ?
        """, (id,))

        keys = [col[0] for col in cursor.description]
        results = [dict(zip(keys, row)) for row in cursor.fetchall()]

        cursor.close()
        conn.close()

        return flask.jsonify(results)

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# ADD
@publisher_bp.route('/publisher/add', methods=['POST'])
def add_publisher():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        INSERT INTO Publisher (PublisherName, PublisherAddress, PublisherPhone)
        VALUES (?, ?, ?)
        """

        cursor.execute(command, (
            data["PublisherName"],
            data["PublisherAddress"],
            data["PublisherPhone"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "them thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# UPDATE
@publisher_bp.route('/publisher/update', methods=['PUT'])
def update_publisher():
    try:
        data = flask.request.json
        conn = get_connection()
        cursor = conn.cursor()

        command = """
        UPDATE Publisher
        SET PublisherName=?, PublisherAddress=?, PublisherPhone=?
        WHERE PublisherID=?
        """

        cursor.execute(command, (
            data["PublisherName"],
            data["PublisherAddress"],
            data["PublisherPhone"],
            data["PublisherID"]
        ))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "cap nhat thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500

# DELETE
@publisher_bp.route('/publisher/delete/<id>', methods=['DELETE'])
def delete_publisher(id):
    try:
        conn = get_connection()
        cursor = conn.cursor()

        cursor.execute("DELETE FROM Publisher WHERE PublisherID = ?", (id,))
        conn.commit()

        cursor.close()
        conn.close()

        return flask.jsonify({"mess": "xoa thanh cong"})

    except Exception as e:
        return flask.jsonify({"mess": str(e)}), 500