# AutoQA Runtime 0.1

Український огляд усіх класів, приклад використання та Mermaid-діаграми:
[Як працює AutoQA Runtime](ARCHITECTURE.uk.md).

Embedded package for Unity 6000.3. Core has no Editor, transport, model or game
dependency. UGUI currently supports Button and TMP_InputField in Direct mode.
Toggle, dropdown, sliders and scrolling are future additions; drag and drop is
outside the current scope.

## Runtime API

Create RuntimeFacade on Unity's main thread, supplying an interaction provider
and optional context/readiness/oracle providers. BeginSession, then repeatedly
CaptureObservation. Pick an offered actionId and Execute an ActionCommand with
the observation's sessionId, observationId and revision. Capture again after
execution and wait for readiness. GetCommandStatus retrieves a stored result.
EndSession/Dispose releases the session. Dispose the facade when its owner dies.

Calls must run on the creating Unity main thread. Future transports must marshal
calls there. A session survives scene changes if its owner does. Restarting Play
Mode creates a new session; stale commands must not be replayed.

JSON uses Unity JsonUtility and public serializable DTOs. gameContext is a list
of string key/value facts, not arbitrary serialized game components. This is
schema version 1. An explicit future schema version can extend the value model.

Readiness defaults to unavailable when no provider exists. A generic UI-only
consumer may supply its own explicit readiness policy; the package does not
infer completion of arbitrary game initialization from visible controls.

Input candidates require ITestDataProvider fixtures; the package never invents
values. Setting a TMP field uses its text setter (onValueChanged when changed)
and then onEndEdit. It does not simulate focus, typing, onSubmit, or pointer
events. Button execution invokes onClick after checking active/interactable.
Direct mode does not prove raycast accessibility or user clickability.

Command IDs are deduplicated for the entire session, including rejected results.
Use a new ID after correcting a rejected command. A repeated ID with another
payload is rejected. A synchronous callback cannot be interrupted: a deadline
overrun is reported after the callback and its effects may already have happened.
Completed is an operation result, not a passing test.

## Optional descriptions

QAContext can be attached to a screen, popup, control or scene object. Description
and SemanticKey are optional. Standard actions work without this component.
Context records refer to owner/parent target IDs, preserving structural scope.
Inactive objects, disabled canvases and zero-alpha CanvasGroups are excluded.
Custom visibility behavior should be represented by a game integration provider.
The descriptor is not a readiness signal or an oracle.

The future Host reads one Markdown game description by configured path, separately
from the test goal. Runtime does not read external files or require a planner.

## Tests

Add com.maze2d.autoqa to Packages/manifest.json testables (already done in this
project). In Window > General > Test Runner run AutoQA.Tests in EditMode and
PlayMode. The PlayMode test constructs and destroys a compact UI fixture at
runtime, so no test scene needs to be installed in consumer projects.
