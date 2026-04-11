#!/usr/bin/env python3
import json
import time
import urllib.error
import urllib.request

BASE = "http://localhost:5050/api"


def request(method: str, path: str, payload: dict | None = None, token: str | None = None) -> tuple[int, dict | None]:
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"

    body = None
    if payload is not None:
        body = json.dumps(payload).encode("utf-8")

    req = urllib.request.Request(f"{BASE}{path}", data=body, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=20) as response:
            raw = response.read().decode("utf-8")
            data = json.loads(raw) if raw else None
            return response.status, data
    except urllib.error.HTTPError as ex:
        raw = ex.read().decode("utf-8")
        data = json.loads(raw) if raw else None
        return ex.code, data


def wait_for_backend() -> None:
    for _ in range(30):
        status, _ = request("GET", "/articles?pageNumber=1&pageSize=1")
        if status == 200:
            return
        time.sleep(1)
    raise RuntimeError("Backend readiness failed")


def main() -> None:
    wait_for_backend()

    email = f"smoke{int(time.time())}@example.com"
    password = "Pass123!"

    results: dict[str, object] = {}

    status, payload = request("POST", "/auth/register", {"email": email, "password": password})
    results["register"] = status
    if status != 200:
        raise RuntimeError(f"register failed: {status} {payload}")

    status, payload = request("POST", "/auth/login", {"email": email, "password": password})
    results["login"] = status
    if status != 200 or payload is None:
        raise RuntimeError(f"login failed: {status} {payload}")

    access_token = payload["accessToken"]
    refresh_token = payload["refreshToken"]

    status, payload = request("POST", "/articles", {"title": "Smoke title", "content": "Smoke content"}, access_token)
    results["create"] = status
    if status != 201 or payload is None:
        raise RuntimeError(f"create failed: {status} {payload}")

    article_id = payload["id"]

    status, payload = request("GET", "/articles?pageNumber=1&pageSize=10")
    results["list"] = status
    if status != 200:
        raise RuntimeError(f"list failed: {status} {payload}")

    status, payload = request("GET", f"/articles/{article_id}")
    results["getById"] = status
    if status != 200:
        raise RuntimeError(f"getById failed: {status} {payload}")

    status, payload = request(
        "PUT",
        f"/articles/{article_id}",
        {"title": "Smoke title updated", "content": "Smoke content updated"},
        access_token,
    )
    results["update"] = status
    if status != 200:
        raise RuntimeError(f"update failed: {status} {payload}")

    status, payload = request("POST", "/auth/refresh-token", {"email": email, "refreshToken": refresh_token})
    results["refresh"] = status
    if status != 200:
        raise RuntimeError(f"refresh failed: {status} {payload}")

    status, payload = request("DELETE", f"/articles/{article_id}", token=access_token)
    results["delete"] = status
    if status != 200:
        raise RuntimeError(f"delete failed: {status} {payload}")

    status, payload = request("GET", f"/articles/{article_id}")
    results["getDeleted"] = status
    if status != 404:
        raise RuntimeError(f"getDeleted expected 404, got: {status} {payload}")

    results["articleId"] = article_id
    results["email"] = email

    print(json.dumps(results, indent=2))
    print("Smoke test OK")


if __name__ == "__main__":
    main()

