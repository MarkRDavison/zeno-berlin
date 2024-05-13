{{- define "helpers.list-api-deployment-env-variables" }}
{{- range $key, $val := .Values.api.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.api.env.secret }}
- name: "BERLIN__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-fanfic-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-api-jobs-deployment-env-variables" }}
{{- range $key, $val := .Values.apijobs.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.apijobs.env.secret }}
- name: "BERLIN__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-fanfic-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-api-orchestrator-deployment-env-variables" }}
{{- range $key, $val := .Values.apiorchestrator.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.apiorchestrator.env.secret }}
- name: "BERLIN__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-fanfic-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-bff-deployment-env-variables" }}
{{- range $key, $val := .Values.bffweb.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.bffweb.env.secret }}
- name: "BERLIN__{{ $key }}"
  valueFrom:
    secretKeyRef:
      name: 'zeno-fanfic-secret'
      key: {{ $key }}
{{- end}}
{{- end }}

{{- define "helpers.list-web-deployment-env-variables" }}
{{- range $key, $val := .Values.web.env.public }}
- name: {{ $key }}
  value: {{ $val | quote }}
{{- end }}
{{- range $key := .Values.web.env.secret }}
- name: {{ $key }}
  valueFrom:
    secretKeyRef:
      name: 'zeno-fanfic-secret'
      key: {{ $key }}
{{- end}}
{{- end }}