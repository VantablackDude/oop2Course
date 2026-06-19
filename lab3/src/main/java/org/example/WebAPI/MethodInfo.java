package org.example.WebAPI;

public class MethodInfo {
    private String methodName;
    private String parameters;
    private String returnType;

    public MethodInfo() {}

    public MethodInfo(String methodName, String parameters, String returnType) {
        this.methodName = methodName;
        this.parameters = parameters;
        this.returnType = returnType;
    }

    // Геттеры и сеттеры
    public String getMethodName() { return methodName; }
    public void setMethodName(String methodName) { this.methodName = methodName; }
    public String getParameters() { return parameters; }
    public void setParameters(String parameters) { this.parameters = parameters; }
    public String getReturnType() { return returnType; }
    public void setReturnType(String returnType) { this.returnType = returnType; }
}