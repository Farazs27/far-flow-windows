#!/usr/bin/env python3
"""Validate this planning repository; this is not a Windows application test."""
from pathlib import Path
import csv
import hashlib
import json
import re

ROOT = Path(__file__).resolve().parents[1]

def main():
    required = [
        "README.md", "LICENSE", "THIRD_PARTY_NOTICES.md",
        "docs/IMPLEMENTATION-PLAN.md", "docs/ARCHITECTURE.md",
        "docs/COMPATIBILITY-AND-MODELS.md", "docs/MAC-TO-WINDOWS-AUDIT.md",
        "docs/VALIDATION-AND-RELEASE.md", "docs/PRIVACY-AND-SECURITY.md",
        "docs/REPOSITORY-STRUCTURE.md", "research/00-brief.md",
        "research/25-arbitration.md", "research/30-final-report.md", "research/31-sources.md",
    ]
    for name in required:
        assert (ROOT / name).is_file(), f"Missing {name}"
    json_files = [p for p in ROOT.rglob("*.json") if ".git" not in p.parts and ".tools" not in p.parts]
    for path in json_files:
        json.loads(path.read_text())
    ledger = json.loads((ROOT / "planning/claim-ledger.json").read_text())
    assert len(ledger) == 45 and len({x["id"] for x in ledger}) == 45
    counts = {key: sum(x["verdict"] == key for x in ledger) for key in ("CONFIRMED", "PARTIAL", "DISPUTED", "UNVERIFIABLE")}
    status = json.loads((ROOT / "planning/evidence-status.json").read_text())
    assert status["research"]["validation"] == counts
    assert all(value == "NOT RUN" for value in status["windows"].values()), "Do not claim unexecuted Windows evidence"
    for n in range(1, 5):
        assert (ROOT / f"research/10-findings-{n}.md").is_file()
        assert (ROOT / f"research/20-validation-{n}.md").is_file()
    provenance = json.loads((ROOT / "planning/asset-provenance.json").read_text())
    for item in provenance:
        path = ROOT / item["path"]
        assert path.stat().st_size == item["bytes"], item["path"]
        assert hashlib.sha256(path.read_bytes()).hexdigest() == item["sha256"], item["path"]
    for name in ("backlog.csv", "acceptance-matrix.csv"):
        with (ROOT / "planning" / name).open(newline="") as stream:
            rows = list(csv.DictReader(stream))
        assert rows and len({x["id"] for x in rows}) == len(rows), name
        if name == "acceptance-matrix.csv":
            assert all(row["status"] == "NOT RUN" for row in rows)
    broken = []
    for path in ROOT.rglob("*.md"):
        if ".git" in path.parts or ".tools" in path.parts:
            continue
        body = path.read_text()
        targets = re.findall(r'!?\[[^\]]*\]\(([^)]+)\)', body)
        targets += re.findall(r'(?:src|href)="([^"]+)"', body)
        for target in targets:
            if "://" in target or target.startswith("#"):
                continue
            clean = target.split("#")[0].strip("<>")
            if clean and not (path.parent / clean).exists():
                broken.append(f"{path.relative_to(ROOT)} -> {target}")
    assert not broken, "Broken local links: " + "; ".join(broken)
    print(f"Planning checks passed: {len(ledger)} reviewed claims, {len(json_files)} JSON files, {len(provenance)} assets; Windows evidence remains NOT RUN.")

if __name__ == "__main__":
    main()
