# Contributing to cuvis.csharp

This wrapper is no longer actively maintained.
It is still kept building against new cuvis SDK releases, so it can be brought back if needed.
For bug reports and questions use [GitHub Issues](https://github.com/cubert-hyperspectral/cuvis.csharp/issues).

## Branch model

| Branch | Role |
| --- | --- |
| `main` | Trunk, and the latest wrapper state for the latest released cuvis SDK. Never receives direct pushes. |
| `feature/*` | One branch per change, cut from `main`, merged back into `main` by pull request. |
| `release/vX.Y` | Maintenance branch for an older SDK line that still receives wrapper revisions. Cut from the corresponding tag on demand. |

```
feature/*  ->  main  (tag vX.Y.Z)
```

A release is a `v*` tag on a commit of `main`; not every commit on `main` is a release.

The `develop` branch is retired and must not be used as a base or merge target any more.
It held no commits beyond `main` when it was retired.

## Submodules

`cuvis.swig` is a submodule shared with the other cuvis wrappers.
A change to the interface layer goes into a pull request on [cuvis.swig](https://github.com/cubert-hyperspectral/cuvis.swig) first; this repository then moves the submodule pointer to the merged commit.
